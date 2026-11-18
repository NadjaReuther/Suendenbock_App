using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.ViewModels;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Die Handy-App (/App). Nutzt dieselbe Datenbank wie die Website,
    /// zeigt sie aber in einer eigenen, handyfreundlichen Oberfläche:
    /// Start · Termine · Forum · Charakter · Post, dazu Profil und Wiki-Suche.
    /// </summary>
    [Authorize]
    public class AppController : Controller
    {
        private const int FeedDays = 30;

        private static readonly string[] MonthShort =
            { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageUploadService _imageUploadService;
        private readonly IAchievementService _achievementService;
        private readonly ILogger<AppController> _logger;

        public AppController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IImageUploadService imageUploadService,
            IAchievementService achievementService,
            ILogger<AppController> logger)
        {
            _context = context;
            _userManager = userManager;
            _imageUploadService = imageUploadService;
            _achievementService = achievementService;
            _logger = logger;
        }

        // ================================================================
        // START
        // ================================================================

        // GET /App
        public async Task<IActionResult> Index()
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "start");

            var vm = new AppStartViewModel
            {
                DisplayName = DisplayName(user),
                NextEvent = (await LoadEventsAsync(user, onlyNext: true)).FirstOrDefault(),
                Payment = await LoadPaymentAsync(user),
                Feed = await BuildFeedAsync(user)
            };

            return View(vm);
        }

        // GET /App/Neuigkeiten  (für das Pop-up "Seit deinem letzten Besuch")
        [HttpGet]
        public async Task<IActionResult> Neuigkeiten()
        {
            var user = await GetUserAsync();
            if (user == null) return Unauthorized();
            return Json(await BuildFeedAsync(user));
        }

        // ================================================================
        // TERMINE
        // ================================================================

        // GET /App/Termine
        public async Task<IActionResult> Termine()
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "termine");

            return View(new AppTermineViewModel { Events = await LoadEventsAsync(user, onlyNext: false) });
        }

        // ================================================================
        // FORUM
        // ================================================================

        // GET /App/Forum?kategorie=Regeln
        public async Task<IActionResult> Forum(string? kategorie)
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "forum");

            var categories = await _context.ForumCategories
                .OrderBy(c => c.SortOrder)
                .Select(c => new ForumCategoryViewModel { Id = c.Id, Name = c.Name, Icon = c.Icon })
                .ToListAsync();

            var query = _context.ForumThreads
                .Include(t => t.Category)
                .Include(t => t.Replies)
                .Include(t => t.AuthorUser)
                .Include(t => t.AuthorCharacter)
                .Where(t => !t.IsArchived)
                .AsSplitQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(kategorie))
                query = query.Where(t => t.Category.Name == kategorie);

            var threads = await query.ToListAsync();

            var items = threads
                .Select(t =>
                {
                    var replies = t.Replies.Where(r => !r.IsArchived).ToList();
                    return new AppThreadItem
                    {
                        Id = t.Id,
                        Title = t.Title,
                        CategoryName = t.Category.Name,
                        CategoryIcon = t.Category.Icon,
                        AuthorName = AuthorDisplayName(t.AuthorUser, t.AuthorName),
                        ReplyCount = replies.Count,
                        IsPinned = t.IsPinned,
                        LastActivity = replies.Any() ? replies.Max(r => r.CreatedAt) : t.CreatedAt
                    };
                })
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.LastActivity)
                .ToList();

            return View(new AppForumViewModel
            {
                Categories = categories,
                ActiveCategory = kategorie,
                Threads = items
            });
        }

        // GET /App/Thema/5
        public async Task<IActionResult> Thema(int id)
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "forum");

            var thread = await _context.ForumThreads
                .Include(t => t.Category)
                .Include(t => t.AuthorUser)
                .Include(t => t.AuthorCharacter)
                .Include(t => t.Replies.Where(r => !r.IsArchived))
                    .ThenInclude(r => r.AuthorUser)
                .Include(t => t.Replies.Where(r => !r.IsArchived))
                    .ThenInclude(r => r.AuthorCharacter)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsArchived);

            if (thread == null) return NotFound();

            return View(new AppThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                CategoryName = thread.Category.Name,
                AuthorName = AuthorDisplayName(thread.AuthorUser, thread.AuthorName),
                CreatedAt = thread.CreatedAt,
                Content = thread.Content,
                Replies = thread.Replies
                    .OrderBy(r => r.CreatedAt)
                    .Select(r => new AppReplyItem
                    {
                        AuthorName = AuthorDisplayName(r.AuthorUser, r.AuthorName),
                        IsMine = r.AuthorUserId == user.Id,
                        CreatedAt = r.CreatedAt,
                        Content = r.Content
                    })
                    .ToList()
            });
        }

        // GET /App/ForumSuche?q=pokus  (für die Lupe)
        [HttpGet]
        public async Task<IActionResult> ForumSuche(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Json(Array.Empty<object>());

            var term = q.Trim();
            var results = await _context.ForumThreads
                .Include(t => t.Category)
                .Where(t => !t.IsArchived &&
                            (EF.Functions.Like(t.Title, $"%{term}%") || EF.Functions.Like(t.Content, $"%{term}%")))
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.CreatedAt)
                .Take(10)
                .Select(t => new
                {
                    id = t.Id,
                    name = t.Title,
                    type = "forum",
                    url = $"/App/Thema/{t.Id}",
                    subtitle = t.Category.Name
                })
                .ToListAsync();

            return Json(results);
        }

        // ================================================================
        // WIKI-EINTRAG (Leseansicht für die Suche)
        // ================================================================

        // GET /App/Eintrag?type=glossary&id=3
        [HttpGet]
        public async Task<IActionResult> Eintrag(string type, int id)
        {
            AppEntryViewModel? vm = null;

            switch ((type ?? "").ToLower())
            {
                case "character":
                {
                    var c = await _context.Characters
                        .Include(x => x.Details)
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (c != null)
                    {
                        vm = new AppEntryViewModel
                        {
                            TypeLabel = c.IsCompanion ? "Begleiter" : "Charakter",
                            Title = $"{c.Vorname} {c.Nachname}".Trim(),
                            Subtitle = string.Join(" · ", new[] { c.Rufname, c.Details?.Beruf }
                                .Where(s => !string.IsNullOrWhiteSpace(s))),
                            ImagePath = c.ImagePath,
                            Quote = c.Details?.quote,
                            QuoteBy = c.Details?.urheber,
                            DescriptionHtml = c.Details?.Description,
                            FullPageUrl = $"/Character/CharacterSheet/{c.Id}"
                        };
                    }
                    break;
                }
                case "guild":
                {
                    var g = await _context.Guilds.FirstOrDefaultAsync(x => x.Id == id);
                    if (g != null)
                    {
                        vm = new AppEntryViewModel
                        {
                            TypeLabel = "Gilde",
                            Title = g.Name,
                            ImagePath = g.ImagePath,
                            Quote = g.quote,
                            QuoteBy = g.urheber,
                            DescriptionHtml = g.Description,
                            FullPageUrl = $"/Guild/GuildSheet/{g.Id}"
                        };
                    }
                    break;
                }
                case "infanterie":
                {
                    var i = await _context.Infanterien.FirstOrDefaultAsync(x => x.Id == id);
                    if (i != null)
                    {
                        vm = new AppEntryViewModel
                        {
                            TypeLabel = "Infanterie",
                            Title = $"{i.Bezeichnung}. Infanterie",
                            Subtitle = string.IsNullOrWhiteSpace(i.Sitz) ? null : $"Sitz: {i.Sitz}",
                            ImagePath = i.ImagePath,
                            DescriptionHtml = i.description,
                            FullPageUrl = $"/Infanterie/Overview?id={i.Id}"
                        };
                    }
                    break;
                }
                case "monster":
                {
                    var m = await _context.Monsters.FirstOrDefaultAsync(x => x.Id == id);
                    if (m != null)
                    {
                        // Noch nicht begegnete Monster bleiben verborgen (wie im Bestiarium)
                        var known = m.meet || m.encounter || m.perfected;
                        vm = new AppEntryViewModel
                        {
                            TypeLabel = "Monster",
                            Title = known ? m.Name : "Unbekanntes Wesen",
                            ImagePath = known ? m.ImagePath : null,
                            DescriptionHtml = known ? (m.ProcessedDescription ?? m.Description) : null,
                            Notice = known ? null : "Diesem Wesen seid ihr noch nicht begegnet.",
                            FullPageUrl = $"/Monster/Overview?monsterId={m.Id}"
                        };
                    }
                    break;
                }
                case "glossary":
                case "magicclass":
                {
                    var e = await _context.GlossaryEntries.FirstOrDefaultAsync(x => x.Id == id);
                    if (e != null)
                    {
                        vm = new AppEntryViewModel
                        {
                            TypeLabel = e.EntityType switch
                            {
                                "MagicClass" => "Magieklasse",
                                "Obermagie" => "Obermagie",
                                null or "" => "Glossar",
                                _ => e.EntityType
                            },
                            Title = e.Title,
                            Subtitle = e.Category,
                            ImagePath = e.ImagePath,
                            DescriptionHtml = e.Description,
                            FullPageUrl = $"/Glossary/Show/{e.Id}"
                        };
                    }
                    break;
                }
            }

            if (vm == null) return NotFound();
            return PartialView("_Eintrag", vm);
        }

        // ================================================================
        // CHARAKTER
        // ================================================================

        // GET /App/Charakter oder /App/Charakter/12
        public async Task<IActionResult> Charakter(int? id)
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "charakter");

            var myCharacters = await _context.Characters
                .Where(c => c.UserId == user.Id)
                .OrderBy(c => c.IsCompanion)
                .ThenBy(c => c.Vorname)
                .ToListAsync();

            if (!myCharacters.Any())
            {
                return View("KeinCharakter");
            }

            // Ohne Angabe: der eigene Hauptcharakter (Begleiter stehen durch die Sortierung hinten).
            // Die frühere Spalte CurrentCharacterId gibt es seit dem Rückbau des Tenant-Systems nicht mehr.
            var selectedId = id ?? myCharacters.First().Id;

            // Nur eigene Charaktere
            if (!myCharacters.Any(c => c.Id == selectedId))
                return Forbid();

            var character = await _context.Characters
                .Include(c => c.Details)
                .Include(c => c.Affiliation)
                .Include(c => c.CharacterMagicClasses).ThenInclude(m => m.MagicClass)
                .Include(c => c.CharacterMagicClasses).ThenInclude(m => m.MagicClassSpecialization)
                .AsSplitQuery()
                .FirstAsync(c => c.Id == selectedId);

            var vm = new AppCharacterViewModel
            {
                Id = character.Id,
                MyCharacters = myCharacters
                    .Select(c => new AppOption
                    {
                        Id = c.Id,
                        Name = $"{c.Vorname} {c.Nachname}".Trim() + (c.IsCompanion ? " (Begleiter)" : "")
                    })
                    .ToList(),
                Vorname = character.Vorname,
                Nachname = character.Nachname,
                Rufname = character.Rufname,
                Geburtsdatum = character.Geburtsdatum,
                ImagePath = character.ImagePath,
                IsCompanion = character.IsCompanion,
                Beruf = character.Details?.Beruf,
                BodyHeight = character.Details?.BodyHeight,
                Quote = character.Details?.quote,
                Urheber = character.Details?.urheber,
                Description = character.Details?.Description,
                StandId = character.Details?.StandId,
                BlutgruppeId = character.Details?.BlutgruppeId,
                HausId = character.Details?.HausId,
                HerkunftslandId = character.Details?.HerkunftslandId,
                GuildId = character.Affiliation?.GuildId,
                RegimentId = character.Affiliation?.RegimentId,
                InfanterierangId = character.Affiliation?.InfanterierangId,
                ReligionId = character.Affiliation?.ReligionId,
                Magic = character.CharacterMagicClasses
                    .Where(m => m.MagicClass != null)
                    .Select(m => m.MagicClassSpecialization != null
                        ? $"{m.MagicClass!.Bezeichnung} · {m.MagicClassSpecialization.Name}"
                        : m.MagicClass!.Bezeichnung)
                    .ToList(),
                Staende = await _context.Staende.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Blutgruppen = await _context.Blutgruppen.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Haeuser = await _context.Haeuser.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Herkunftslaender = await _context.Herkunftslaender.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Guilds = await _context.Guilds.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Regiments = await _context.Regiments.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Infanterieraenge = await _context.Infanterieraenge.OrderBy(x => x.Name).Select(x => new AppOption { Id = x.Id, Name = x.Name }).ToListAsync(),
                Religions = await _context.Religions.OrderBy(x => x.Type).Select(x => new AppOption { Id = x.Id, Name = x.Type }).ToListAsync()
            };

            return View(vm);
        }

        // POST /App/CharakterSpeichern
        // Magie, Lebenspunkte und Pokus sind hier bewusst nicht dabei (nur Spielleitung).
        [HttpPost, ValidateAntiForgeryToken]
        [RequestSizeLimit(15 * 1024 * 1024)]
        public async Task<IActionResult> CharakterSpeichern(AppCharacterSaveModel model, IFormFile? bild)
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();

            var character = await _context.Characters
                .Include(c => c.Details)
                .Include(c => c.Affiliation)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (character == null) return NotFound();

            // Nur eigene Charaktere (Gott darf alles, wie auf der Website)
            if (character.UserId != user.Id && !User.IsInRole("Gott"))
                return Forbid();

            if (string.IsNullOrWhiteSpace(model.Vorname) || string.IsNullOrWhiteSpace(model.Rufname))
            {
                TempData["AppError"] = "Vorname und Rufname dürfen nicht leer sein.";
                return RedirectToAction(nameof(Charakter), new { id = character.Id });
            }

            character.Vorname = model.Vorname.Trim();
            character.Nachname = (model.Nachname ?? "").Trim();
            character.Rufname = model.Rufname.Trim();
            character.Geburtsdatum = model.Geburtsdatum;

            if (character.Details == null)
            {
                character.Details = new CharacterDetails { CharacterId = character.Id };
                _context.CharacterDetails.Add(character.Details);
            }
            character.Details.Beruf = model.Beruf;
            character.Details.BodyHeight = model.BodyHeight;
            character.Details.quote = model.Quote;
            character.Details.urheber = model.Urheber;
            character.Details.Description = model.Description;
            character.Details.StandId = model.StandId;
            character.Details.BlutgruppeId = model.BlutgruppeId;
            character.Details.HausId = model.HausId;
            character.Details.HerkunftslandId = model.HerkunftslandId;

            if (character.Affiliation == null)
            {
                character.Affiliation = new CharacterAffiliation { CharacterId = character.Id };
                _context.CharacterAffiliations.Add(character.Affiliation);
            }
            character.Affiliation.GuildId = model.GuildId;
            character.Affiliation.RegimentId = model.RegimentId;
            character.Affiliation.InfanterierangId = model.InfanterierangId;
            character.Affiliation.ReligionId = model.ReligionId;

            if (bild != null && bild.Length > 0)
            {
                try
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    character.ImagePath = await _imageUploadService.UploadImageAsync(
                        bild, "characters", $"{character.Vorname}_{character.Nachname}_{timestamp}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Bild-Upload aus der App fehlgeschlagen");
                    TempData["AppError"] = $"Das Bild konnte nicht hochgeladen werden: {ex.Message}";
                }
            }

            await _context.SaveChangesAsync();

            try
            {
                await _achievementService.CheckCharacterAchievements(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Achievement-Check nach App-Speichern fehlgeschlagen");
            }

            TempData["AppSuccess"] = "Gespeichert ✓";
            return RedirectToAction(nameof(Charakter), new { id = character.Id });
        }

        // ================================================================
        // POST (Postfach)
        // ================================================================

        // GET /App/Post  bzw. /App/Post?ordner=gesendet
        public async Task<IActionResult> Post(string? ordner)
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "post");

            var sent = ordner == "gesendet";
            var isStaff = User.IsInRole("Gott") || User.IsInRole("Moderator");
            var myCharacterIds = await MyCharacterIdsAsync(user);

            var query = _context.Messages.Include(m => m.SenderCharacter).Include(m => m.ReceiverCharacter).AsQueryable();
            query = sent
                ? query.Where(m => !m.IsDeletedBySender &&
                                   (m.SenderUserId == user.Id ||
                                    (m.SenderCharacterId != null && myCharacterIds.Contains(m.SenderCharacterId.Value))))
                : query.Where(m => !m.IsDeletedByReceiver &&
                                   (m.ReceiverUserId == user.Id ||
                                    (m.ReceiverCharacterId != null && myCharacterIds.Contains(m.ReceiverCharacterId.Value))));

            var allMessages = await query.OrderByDescending(m => m.SentAt).ToListAsync();

            // Für "Gesendet": Namen von Gott/Moderator als Empfänger
            var receiverUserIds = allMessages.Where(m => m.ReceiverUserId != null).Select(m => m.ReceiverUserId!).Distinct().ToList();
            var receiverUsers = await _context.Users
                .Where(u => receiverUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => DisplayName(u));

            var list = allMessages
                .GroupBy(m => m.ThreadId)
                .Select(g =>
                {
                    var latest = g.OrderByDescending(m => m.SentAt).First();
                    return new MessageListItem
                    {
                        Id = latest.Id,
                        ThreadId = latest.ThreadId,
                        OtherCharacterName = sent ? ReceiverName(latest, receiverUsers) : SenderName(latest),
                        Subject = latest.Subject,
                        ContentPreview = Shorten(StripHtml(latest.Content), 110),
                        SentAt = latest.SentAt,
                        IsRead = sent || latest.IsRead,
                        MessageCount = g.Count(),
                        UnreadCount = sent ? 0 : g.Count(m => !m.IsRead)
                    };
                })
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(new AppPostViewModel
            {
                Messages = list,
                UnreadCount = sent ? 0 : allMessages.Count(m => !m.IsRead),
                HasCharacter = myCharacterIds.Any() || isStaff,
                IsSentFolder = sent
            });
        }

        // GET /App/Nachricht/5  (Gesprächsverlauf, markiert als gelesen)
        public async Task<IActionResult> Nachricht(int id)
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "post");

            var myCharacterIds = await MyCharacterIdsAsync(user);
            bool IsMineSent(Message m) => m.SenderUserId == user.Id ||
                                          (m.SenderCharacterId != null && myCharacterIds.Contains(m.SenderCharacterId.Value));
            bool IsMineReceived(Message m) => m.ReceiverUserId == user.Id ||
                                              (m.ReceiverCharacterId != null && myCharacterIds.Contains(m.ReceiverCharacterId.Value));

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null) return NotFound();

            // Nur Absender oder Empfänger (wie auf der Website)
            var isSender = IsMineSent(message);
            var isReceiver = IsMineReceived(message);
            if (!isSender && !isReceiver) return Forbid();
            if ((isSender && !isReceiver && message.IsDeletedBySender) || (isReceiver && !isSender && message.IsDeletedByReceiver))
                return NotFound();

            var thread = await _context.Messages
                .Include(m => m.SenderCharacter)
                .Include(m => m.ReceiverCharacter)
                .Where(m => m.ThreadId == message.ThreadId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Alles, was ich in diesem Gespräch bekommen habe, ist jetzt gelesen
            var unread = thread.Where(m => !m.IsRead && IsMineReceived(m)).ToList();
            if (unread.Any())
            {
                foreach (var m in unread) m.IsRead = true;
                await _context.SaveChangesAsync();
                await SetShellAsync(user, "post");   // Zähler in der Leiste aktualisieren
            }

            var receiverUserIds = thread.Where(m => m.ReceiverUserId != null).Select(m => m.ReceiverUserId!).Distinct().ToList();
            var receiverUsers = await _context.Users
                .Where(u => receiverUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => DisplayName(u));

            // Antwort an: Absender der letzten Nachricht, die nicht von mir ist.
            // Gibt es keine (ich habe das Gespräch begonnen), an den ursprünglichen Empfänger.
            var lastFromOther = thread.LastOrDefault(m => !IsMineSent(m));
            var first = thread.First();
            var vm = new AppMessageThreadViewModel
            {
                Id = message.Id,
                Subject = first.Subject,
                ParentMessageId = thread.Last().Id,
                ReplySubject = first.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase) ? first.Subject : "Re: " + first.Subject,
                Messages = thread
                    .Where(m => !(IsMineReceived(m) && !IsMineSent(m) && m.IsDeletedByReceiver))
                    .Select(m => new AppMessageItem
                    {
                        SenderName = SenderName(m),
                        IsMine = IsMineSent(m),
                        SentAt = m.SentAt,
                        Content = m.Content
                    })
                    .ToList()
            };

            if (lastFromOther != null)
            {
                vm.OtherName = SenderName(lastFromOther);
                if (lastFromOther.SenderType == MessageSenderType.Character && lastFromOther.SenderCharacterId.HasValue)
                    vm.ReplyCharacterId = lastFromOther.SenderCharacterId;
                else if (!string.IsNullOrEmpty(lastFromOther.SenderUserId))
                {
                    vm.ReplyUserId = lastFromOther.SenderUserId;
                    vm.ReplyType = (int)lastFromOther.SenderType;
                }
            }
            else
            {
                vm.OtherName = ReceiverName(first, receiverUsers);
                if (first.ReceiverCharacterId.HasValue)
                    vm.ReplyCharacterId = first.ReceiverCharacterId;
                else if (!string.IsNullOrEmpty(first.ReceiverUserId))
                {
                    vm.ReplyUserId = first.ReceiverUserId;
                    vm.ReplyType = (int)(first.ReceiverType ?? MessageSenderType.Gott);
                }
            }

            vm.CanReply = vm.ReplyCharacterId.HasValue || !string.IsNullOrEmpty(vm.ReplyUserId);
            return View(vm);
        }

        // GET /App/NeueNachricht
        public async Task<IActionResult> NeueNachricht()
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "post");

            var isStaff = User.IsInRole("Gott") || User.IsInRole("Moderator");
            var myCharacterIds = await MyCharacterIdsAsync(user);
            var vm = new AppComposeViewModel();

            if (!isStaff)
            {
                // Spieler schreiben als ihr Charakter (wie auf der Website: der erste zugeordnete)
                var senderCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (senderCharacter == null)
                {
                    TempData["AppError"] = "Für Nachrichten braucht dein Konto einen Charakter.";
                    return RedirectToAction(nameof(Post));
                }
                vm.SenderName = senderCharacter.Vorname;
            }

            // Charaktere mit Spieler (nicht die eigenen)
            var characters = await _context.Characters
                .Where(c => c.UserId != null && c.UserId != "" && !myCharacterIds.Contains(c.Id))
                .OrderBy(c => c.Vorname)
                .Select(c => new { c.Id, c.Vorname, c.Nachname })
                .ToListAsync();
            vm.Receivers.AddRange(characters.Select(c => new AppReceiverOption
            {
                Value = $"char:{c.Id}",
                Name = $"{c.Vorname} {c.Nachname}".Trim(),
                Group = "Charaktere"
            }));

            // Spielleitung: Götter und Moderatoren (nicht man selbst)
            foreach (var (role, type) in new[] { ("Gott", MessageSenderType.Gott), ("Moderator", MessageSenderType.Moderator) })
            {
                var staff = await _userManager.GetUsersInRoleAsync(role);
                vm.Receivers.AddRange(staff
                    .Where(u => u.Id != user.Id)
                    .OrderBy(u => DisplayName(u))
                    .Select(u => new AppReceiverOption
                    {
                        Value = $"user:{u.Id}:{(int)type}",
                        Name = $"{DisplayName(u)} ({role})",
                        Group = "Spielleitung"
                    }));
            }

            return View(vm);
        }

        // ================================================================
        // PROFIL
        // ================================================================

        // GET /App/Profil
        public async Task<IActionResult> Profil()
        {
            var user = await GetUserAsync();
            if (user == null) return Challenge();
            await SetShellAsync(user, "profil");

            ViewBag.Email = user.Email ?? user.UserName;
            ViewBag.ActiveCharacter = await _context.Characters
                .Where(c => c.UserId == user.Id)
                .OrderBy(c => c.IsCompanion)
                .ThenBy(c => c.Vorname)
                .Select(c => c.Vorname + " " + c.Nachname)
                .FirstOrDefaultAsync();

            return View();
        }

        // ================================================================
        // Hilfen
        // ================================================================

        private async Task<ApplicationUser?> GetUserAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return null;

            return await _context.Users
                .Include(u => u.CurrentCharacter)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>Daten für Kopfleiste und Tab-Leiste</summary>
        private async Task SetShellAsync(ApplicationUser user, string activeTab)
        {
            var name = DisplayName(user);
            ViewBag.AppTab = activeTab;
            ViewBag.AppUserName = name;
            ViewBag.AppUserInitial = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpper();

            var myCharacterIds = await _context.Characters
                .Where(c => c.UserId == user.Id)
                .Select(c => c.Id)
                .ToListAsync();

            ViewBag.AppUnreadMessages = await _context.Messages
                .CountAsync(m => !m.IsRead && !m.IsDeletedByReceiver &&
                                 (m.ReceiverUserId == user.Id ||
                                  (m.ReceiverCharacterId != null && myCharacterIds.Contains(m.ReceiverCharacterId.Value))));
        }

        private static string DisplayName(ApplicationUser user) =>
            !string.IsNullOrWhiteSpace(user.Name) ? user.Name!.Trim() : (user.UserName ?? "").Split('@')[0];

        /// <summary>Echter Name des Autors, sonst wie auf der Website (Charakter oder Login)</summary>
        private static string AuthorDisplayName(ApplicationUser? author, string fallback) =>
            !string.IsNullOrWhiteSpace(author?.Name) ? author!.Name!.Trim() : fallback.Split('@')[0];

        private async Task<List<int>> MyCharacterIdsAsync(ApplicationUser user) =>
            await _context.Characters.Where(c => c.UserId == user.Id).Select(c => c.Id).ToListAsync();

        private static string ReceiverName(Message m, Dictionary<string, string> users)
        {
            if (m.ReceiverCharacter != null) return m.ReceiverCharacter.Vorname;
            if (!string.IsNullOrEmpty(m.ReceiverUserId) && users.TryGetValue(m.ReceiverUserId, out var name)) return name;
            return "Unbekannt";
        }

        private static string SenderName(Message m) =>
            m.SenderType == MessageSenderType.Character
                ? m.SenderCharacter?.Vorname ?? "Unbekannt"
                : m.SenderDisplayName ?? (m.SenderType == MessageSenderType.Gott ? "Gott" : "Moderator");

        private static long ToUnixMs(DateTime dt) => new DateTimeOffset(dt).ToUnixTimeMilliseconds();

        private static string StripHtml(string? html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            var text = Regex.Replace(html, "<[^>]+>", " ");
            text = System.Net.WebUtility.HtmlDecode(text);
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private static string Shorten(string text, int max) =>
            text.Length <= max ? text : text.Substring(0, max).TrimEnd() + " …";

        private AppEventViewModel ToEventViewModel(MonthlyEvent e, ApplicationUser user)
        {
            var myRsvp = e.RSVPs.FirstOrDefault(r => r.UserId == user.Id);
            var myChore = EventNotificationTexts.FindChoreFor(user, e.Chores);

            return new AppEventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Type = e.Type,
                Date = e.Date,
                Month = MonthShort[e.Date.Month - 1],
                Day = e.Date.Day.ToString(),
                When = EventNotificationTexts.FormatWhen(e),
                Description = string.IsNullOrWhiteSpace(e.Description) ? null : e.Description,
                YesCount = e.RSVPs.Count(r => r.Status == "yes"),
                MyRsvp = myRsvp?.Status,
                MyChore = myChore?.ChoreName,
                Chores = e.Chores
                    .OrderBy(c => Array.IndexOf(ChoreNames.All, c.ChoreName))
                    .Select(c => new AppChoreViewModel
                    {
                        Name = c.ChoreName,
                        AssignedTo = string.IsNullOrWhiteSpace(c.AssignedToName) ? "Offen" : c.AssignedToName!,
                        IsMine = myChore != null && c.Id == myChore.Id
                    })
                    .ToList()
            };
        }

        private async Task<List<AppEventViewModel>> LoadEventsAsync(ApplicationUser user, bool onlyNext)
        {
            var today = DateTime.Today;
            var query = _context.MonthlyEvents
                .Include(e => e.RSVPs)
                .Include(e => e.Chores)
                .Where(e => e.Date >= today)
                .OrderBy(e => e.Date)
                .AsSplitQuery();

            var events = onlyNext ? await query.Take(1).ToListAsync() : await query.ToListAsync();
            return events.Select(e => ToEventViewModel(e, user)).ToList();
        }

        private async Task<AppPaymentViewModel?> LoadPaymentAsync(ApplicationUser user)
        {
            // Nur wer Beiträge zahlt (siehe PaymentPeriod), bekommt die Karte
            var players = await PaymentPeriod.GetPayingPlayersAsync(_userManager);
            if (!players.Any(p => p.Id == user.Id)) return null;

            var (year, month) = PaymentPeriod.GetTargetMonth(DateTime.Now);
            var payment = await _context.MonthlyPayments
                .FirstOrDefaultAsync(p => p.Year == year && p.Month == month && p.PlayerName == user.UserName);

            return new AppPaymentViewModel
            {
                MonthName = PaymentPeriod.GetMonthName(month),
                IsPaid = payment?.Status == "paid",
                PaymentMethod = payment?.PaymentMethod,
                PaidAt = payment?.PaidAt?.ToString("dd.MM.yyyy")
            };
        }

        /// <summary>
        /// Neuigkeiten der letzten 30 Tage. Was davon "neu seit dem letzten Besuch" ist,
        /// entscheidet die App auf dem Handy (localStorage), damit es keine Datenbank-Änderung braucht.
        /// </summary>
        private async Task<List<AppFeedItem>> BuildFeedAsync(ApplicationUser user)
        {
            var since = DateTime.Now.AddDays(-FeedDays);
            var today = DateTime.Today;
            var feed = new List<AppFeedItem>();

            // Neue Termine (nicht die eigenen)
            var events = await _context.MonthlyEvents
                .Where(e => e.CreatedAt >= since && e.Date >= today && e.CreatedByUserId != user.Id)
                .ToListAsync();
            feed.AddRange(events.Select(e => new AppFeedItem
            {
                Kind = "event", Label = "Neuer Termin", Icon = "event_available",
                Title = e.Title, Text = $"{e.Type} · {EventNotificationTexts.FormatWhen(e)}",
                Url = "/App/Termine", Timestamp = ToUnixMs(e.CreatedAt)
            }));

            // Ungelesene Nachrichten
            var myCharacterIds = await _context.Characters
                .Where(c => c.UserId == user.Id).Select(c => c.Id).ToListAsync();
            var messages = await _context.Messages
                .Include(m => m.SenderCharacter)
                .Where(m => !m.IsRead && !m.IsDeletedByReceiver && m.SentAt >= since &&
                            (m.ReceiverUserId == user.Id ||
                             (m.ReceiverCharacterId != null && myCharacterIds.Contains(m.ReceiverCharacterId.Value))))
                .ToListAsync();
            feed.AddRange(messages.Select(m => new AppFeedItem
            {
                Kind = "msg", Label = "Nachricht", Icon = "mail",
                Title = SenderName(m), Text = m.Subject,
                Url = $"/App/Nachricht/{m.Id}", Timestamp = ToUnixMs(m.SentAt)
            }));

            // News
            var news = await _context.NewsItems.Where(n => n.CreatedAt >= since).ToListAsync();
            feed.AddRange(news.Select(n => new AppFeedItem
            {
                Kind = "news", Label = "News", Icon = string.IsNullOrEmpty(n.Icon) ? "campaign" : n.Icon,
                Title = n.Title, Text = Shorten(StripHtml(n.Excerpt), 90),
                Url = "/Community/News", Timestamp = ToUnixMs(n.CreatedAt)
            }));

            // Umfragen
            var polls = await _context.Polls
                .Where(p => p.Status == "active" && p.CreatedAt >= since && p.CreatedByUserId != user.Id)
                .ToListAsync();
            feed.AddRange(polls.Select(p => new AppFeedItem
            {
                Kind = "poll", Label = "Umfrage", Icon = "poll",
                Title = p.Question, Text = "Stimme jetzt ab",
                Url = "/Community/Polls", Timestamp = ToUnixMs(p.CreatedAt)
            }));

            // Forum: neue Themen und neue Antworten (nicht die eigenen)
            var threads = await _context.ForumThreads
                .Include(t => t.Category)
                .Include(t => t.Replies)
                .Where(t => !t.IsArchived &&
                            ((t.CreatedAt >= since && t.AuthorUserId != user.Id) ||
                             t.Replies.Any(r => !r.IsArchived && r.CreatedAt >= since && r.AuthorUserId != user.Id)))
                .AsSplitQuery()
                .ToListAsync();
            foreach (var t in threads)
            {
                var newReplies = t.Replies
                    .Where(r => !r.IsArchived && r.CreatedAt >= since && r.AuthorUserId != user.Id)
                    .ToList();
                var isNewThread = t.CreatedAt >= since && t.AuthorUserId != user.Id;
                var latest = newReplies.Any() ? newReplies.Max(r => r.CreatedAt) : t.CreatedAt;

                feed.Add(new AppFeedItem
                {
                    Kind = "forum",
                    Label = isNewThread ? "Forum · neues Thema" : "Forum · Antwort",
                    Icon = string.IsNullOrEmpty(t.Category?.Icon) ? "forum" : t.Category!.Icon,
                    Title = t.Title,
                    Text = newReplies.Any()
                        ? $"{newReplies.Count} neue Antwort{(newReplies.Count == 1 ? "" : "en")}"
                        : $"Neues Thema in „{t.Category?.Name}“",
                    Url = $"/App/Thema/{t.Id}",
                    Timestamp = ToUnixMs(latest)
                });
            }

            // Eigener Beitrag bestätigt
            var paid = await _context.MonthlyPayments
                .Where(p => p.PlayerName == user.UserName && p.Status == "paid" && p.PaidAt != null && p.PaidAt >= since)
                .ToListAsync();
            feed.AddRange(paid.Select(p => new AppFeedItem
            {
                Kind = "pay", Label = "Monatsbeitrag", Icon = "check_circle",
                Title = "Beitrag erhalten",
                Text = $"{PaymentPeriod.GetMonthName(p.Month)}" + (string.IsNullOrEmpty(p.PaymentMethod) ? "" : $" · {p.PaymentMethod}"),
                Url = "/App", Timestamp = ToUnixMs(p.PaidAt!.Value)
            }));

            return feed.OrderByDescending(f => f.Timestamp).ToList();
        }
    }
}
