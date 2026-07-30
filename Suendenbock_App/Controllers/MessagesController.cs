using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.ViewModels;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Spieler,Moderator,Gott")] // Nur Spieler, Moderatoren und Götter - KEINE Gäste
    public class MessagesController : BaseController
    {
        public MessagesController(ApplicationDbContext context) : base(context)
        {
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        // GET: /Messages/Inbox
        public async Task<IActionResult> Inbox()
        {
            var userId = GetUserId();
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            List<MessageListItem> messages;
            int unreadCount;

            if (isGott || isModerator)
            {
                // Gott/Moderator: Suche nach ReceiverUserId
                var allMessages = await _context.Messages
                    .Include(m => m.SenderCharacter)
                    .Where(m => m.ReceiverUserId == userId && !m.IsDeletedByReceiver)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();

                // Gruppiere nach ThreadId und zeige nur neueste Nachricht pro Thread
                messages = allMessages
                    .GroupBy(m => m.ThreadId)
                    .Select(g => {
                        var latest = g.OrderByDescending(m => m.SentAt).First();
                        var threadMessages = allMessages.Where(m => m.ThreadId == g.Key).ToList();

                        return new MessageListItem
                        {
                            Id = latest.Id,
                            ThreadId = latest.ThreadId,
                            OtherCharacterName = latest.SenderType == Models.Domain.MessageSenderType.Character
                                ? latest.SenderCharacter?.Vorname ?? "Unbekannt"
                                : latest.SenderDisplayName ?? (latest.SenderType == Models.Domain.MessageSenderType.Gott ? "Gott" : "Moderator"),
                            Subject = latest.Subject,
                            ContentPreview = latest.Content.Length > 100 ? latest.Content.Substring(0, 100) + "..." : latest.Content,
                            SentAt = latest.SentAt,
                            IsRead = latest.IsRead,
                            MessageCount = threadMessages.Count,
                            UnreadCount = threadMessages.Count(m => !m.IsRead)
                        };
                    })
                    .OrderByDescending(m => m.SentAt)
                    .ToList();

                unreadCount = allMessages.Count(m => !m.IsRead);
            }
            else
            {
                // Spieler: Suche nach ReceiverCharacterId
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var allMessages = await _context.Messages
                    .Include(m => m.SenderCharacter)
                    .Where(m => m.ReceiverCharacterId == userCharacter.Id && !m.IsDeletedByReceiver)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();

                // Gruppiere nach ThreadId und zeige nur neueste Nachricht pro Thread
                messages = allMessages
                    .GroupBy(m => m.ThreadId)
                    .Select(g => {
                        var latest = g.OrderByDescending(m => m.SentAt).First();
                        var threadMessages = allMessages.Where(m => m.ThreadId == g.Key).ToList();

                        return new MessageListItem
                        {
                            Id = latest.Id,
                            ThreadId = latest.ThreadId,
                            OtherCharacterName = latest.SenderType == Models.Domain.MessageSenderType.Character
                                ? latest.SenderCharacter?.Vorname ?? "Unbekannt"
                                : latest.SenderDisplayName ?? (latest.SenderType == Models.Domain.MessageSenderType.Gott ? "Gott" : "Moderator"),
                            Subject = latest.Subject,
                            ContentPreview = latest.Content.Length > 100 ? latest.Content.Substring(0, 100) + "..." : latest.Content,
                            SentAt = latest.SentAt,
                            IsRead = latest.IsRead,
                            MessageCount = threadMessages.Count,
                            UnreadCount = threadMessages.Count(m => !m.IsRead)
                        };
                    })
                    .OrderByDescending(m => m.SentAt)
                    .ToList();

                unreadCount = allMessages.Count(m => !m.IsRead);
            }

            var viewModel = new MessagesInboxViewModel
            {
                Messages = messages,
                UnreadCount = unreadCount
            };

            return View(viewModel);
        }

        // GET: /Messages/Sent
        public async Task<IActionResult> Sent()
        {
            var userId = GetUserId();
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            List<MessageListItem> messages;

            if (isGott || isModerator)
            {
                // Gott/Moderator: Suche nach SenderUserId
                var sentMessages = await _context.Messages
                    .Include(m => m.ReceiverCharacter)
                    .Where(m => m.SenderUserId == userId && !m.IsDeletedBySender)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();

                // Gruppiere nach ThreadId und zeige nur neueste Nachricht pro Thread
                messages = sentMessages
                    .GroupBy(m => m.ThreadId)
                    .Select(g => {
                        var latest = g.OrderByDescending(m => m.SentAt).First();
                        var threadMessages = sentMessages.Where(m => m.ThreadId == g.Key).ToList();

                        return new MessageListItem
                        {
                            Id = latest.Id,
                            ThreadId = latest.ThreadId,
                            OtherCharacterName = latest.ReceiverCharacterId.HasValue && latest.ReceiverCharacter != null
                                ? latest.ReceiverCharacter.Vorname
                                : "System",
                            Subject = latest.Subject,
                            ContentPreview = latest.Content.Length > 100 ? latest.Content.Substring(0, 100) + "..." : latest.Content,
                            SentAt = latest.SentAt,
                            IsRead = latest.IsRead,
                            MessageCount = threadMessages.Count,
                            UnreadCount = 0 // Bei gesendeten Nachrichten nicht relevant
                        };
                    })
                    .OrderByDescending(m => m.SentAt)
                    .ToList();
            }
            else
            {
                // Spieler: Suche nach SenderCharacterId
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var sentMessages = await _context.Messages
                    .Include(m => m.ReceiverCharacter)
                    .Where(m => m.SenderCharacterId == userCharacter.Id && !m.IsDeletedBySender)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();

                // Gruppiere nach ThreadId und zeige nur neueste Nachricht pro Thread
                messages = sentMessages
                    .GroupBy(m => m.ThreadId)
                    .Select(g => {
                        var latest = g.OrderByDescending(m => m.SentAt).First();
                        var threadMessages = sentMessages.Where(m => m.ThreadId == g.Key).ToList();

                        return new MessageListItem
                        {
                            Id = latest.Id,
                            ThreadId = latest.ThreadId,
                            OtherCharacterName = latest.ReceiverCharacterId.HasValue && latest.ReceiverCharacter != null
                                ? latest.ReceiverCharacter.Vorname
                                : "System",
                            Subject = latest.Subject,
                            ContentPreview = latest.Content.Length > 100 ? latest.Content.Substring(0, 100) + "..." : latest.Content,
                            SentAt = latest.SentAt,
                            IsRead = latest.IsRead,
                            MessageCount = threadMessages.Count,
                            UnreadCount = 0 // Bei gesendeten Nachrichten nicht relevant
                        };
                    })
                    .OrderByDescending(m => m.SentAt)
                    .ToList();
            }

            var viewModel = new MessagesSentViewModel
            {
                Messages = messages
            };

            return View(viewModel);
        }

        // GET: /Messages/Read/{id}
        public async Task<IActionResult> Read(int id)
        {
            var userId = GetUserId();
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            var message = await _context.Messages
                .Include(m => m.SenderCharacter)
                .Include(m => m.ReceiverCharacter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            bool isSender = false;
            bool isReceiver = false;

            // Prüfe ob User berechtigt ist (Absender oder Empfänger)
            if (isGott || isModerator)
            {
                // Gott/Moderator: Prüfe SenderUserId oder ReceiverUserId
                isSender = message.SenderUserId == userId;
                isReceiver = message.ReceiverUserId == userId;
            }
            else
            {
                // Spieler: Prüfe SenderCharacterId oder ReceiverCharacterId
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                isSender = message.SenderCharacterId == userCharacter.Id;
                isReceiver = message.ReceiverCharacterId == userCharacter.Id;
            }

            if (!isSender && !isReceiver)
            {
                return Forbid();
            }

            // Prüfe ob gelöscht
            if ((isSender && message.IsDeletedBySender) || (isReceiver && message.IsDeletedByReceiver))
            {
                return NotFound();
            }

            // Markiere ALLE ungelesenen Nachrichten im Thread als gelesen (wenn User Empfänger ist)
            if (isReceiver)
            {
                List<Models.Domain.Message> unreadThreadMessages;

                if (isGott || isModerator)
                {
                    // Gott/Moderator: Markiere alle ungelesenen Nachrichten im Thread wo ich Empfänger bin
                    unreadThreadMessages = await _context.Messages
                        .Where(m => m.ThreadId == message.ThreadId
                            && m.ReceiverUserId == userId
                            && !m.IsRead)
                        .ToListAsync();
                }
                else
                {
                    // Spieler: Markiere alle ungelesenen Nachrichten im Thread wo ich Empfänger bin
                    var userChar = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
                    if (userChar != null)
                    {
                        unreadThreadMessages = await _context.Messages
                            .Where(m => m.ThreadId == message.ThreadId
                                && m.ReceiverCharacterId == userChar.Id
                                && !m.IsRead)
                            .ToListAsync();
                    }
                    else
                    {
                        unreadThreadMessages = new List<Models.Domain.Message>();
                    }
                }

                // Markiere alle als gelesen
                foreach (var msg in unreadThreadMessages)
                {
                    msg.IsRead = true;
                }

                if (unreadThreadMessages.Any())
                {
                    await _context.SaveChangesAsync();
                }
            }

            var senderName = message.SenderType == Models.Domain.MessageSenderType.Character
                ? message.SenderCharacter?.Vorname ?? "Unbekannt"
                : message.SenderDisplayName ?? (message.SenderType == Models.Domain.MessageSenderType.Gott ? "Gott" : "Moderator");

            // Empfänger-Name: Kann Charakter oder Gott/Moderator sein
            string receiverName;
            if (message.ReceiverCharacterId.HasValue && message.ReceiverCharacter != null)
            {
                receiverName = message.ReceiverCharacter.Vorname;
            }
            else if (!string.IsNullOrEmpty(message.ReceiverUserId))
            {
                // Empfänger ist Gott/Moderator - hole Username
                var receiverUser = await _context.Users.FindAsync(message.ReceiverUserId);
                var userName = receiverUser?.UserName?.Replace("@suendenbock.lore", "") ?? "System";
                receiverName = message.ReceiverType == Models.Domain.MessageSenderType.Gott
                    ? $"⚡ {userName}"
                    : $"🛡️ {userName}";
            }
            else
            {
                receiverName = "Unbekannt";
            }

            // Lade alle Nachrichten des Threads
            var threadMessages = await _context.Messages
                .Include(m => m.SenderCharacter)
                .Where(m => m.ThreadId == message.ThreadId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            var threadMessageItems = new List<ThreadMessageItem>();
            foreach (var msg in threadMessages)
            {
                bool msgIsSender = false;
                if (isGott || isModerator)
                {
                    msgIsSender = msg.SenderUserId == userId;
                }
                else
                {
                    var userChar = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
                    msgIsSender = userChar != null && msg.SenderCharacterId == userChar.Id;
                }

                var msgSenderName = msg.SenderType == Models.Domain.MessageSenderType.Character
                    ? msg.SenderCharacter?.Vorname ?? "Unbekannt"
                    : msg.SenderDisplayName ?? (msg.SenderType == Models.Domain.MessageSenderType.Gott ? "Gott" : "Moderator");

                threadMessageItems.Add(new ThreadMessageItem
                {
                    Id = msg.Id,
                    SenderName = msgSenderName,
                    Content = msg.Content,
                    SentAt = msg.SentAt,
                    IsSender = msgIsSender,
                    SenderType = msg.SenderType.ToString()
                });
            }

            var viewModel = new MessageDetailViewModel
            {
                Id = message.Id,
                ThreadId = message.ThreadId,
                SenderCharacterName = senderName,
                ReceiverCharacterName = receiverName,
                Subject = message.Subject,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                IsSender = isSender,
                SenderType = message.SenderType.ToString(),
                ThreadMessages = threadMessageItems
            };

            return View(viewModel);
        }

        // GET: /Messages/Compose?replyTo={id}
        public async Task<IActionResult> Compose(int? replyTo)
        {
            var userId = GetUserId();
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            var availableReceivers = new List<ReceiverSelectItem>();

            if (isGott || isModerator)
            {
                // Gott/Moderator: Können an Charaktere MIT Spieler + andere Götter/Moderatoren schreiben

                // 1. Charaktere mit zugewiesenem Spieler
                var characters = await _context.Characters
                    .Where(c => c.UserId != null && c.UserId != "") // Nur Charaktere mit Spieler
                    .OrderBy(c => c.Vorname)
                    .Select(c => new ReceiverSelectItem
                    {
                        Type = "Character",
                        CharacterId = c.Id,
                        DisplayName = c.Vorname
                    })
                    .ToListAsync();

                availableReceivers.AddRange(characters);

                // 2. Alle Götter
                var goetter = await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Gott")))
                    .Where(u => u.Id != userId) // Nicht sich selbst
                    .OrderBy(u => u.UserName)
                    .Select(u => new ReceiverSelectItem
                    {
                        Type = "Gott",
                        UserId = u.Id,
                        DisplayName = "⚡ " + (u.UserName ?? "Gott").Replace("@suendenbock.lore", "")
                    })
                    .ToListAsync();

                availableReceivers.AddRange(goetter);

                // 3. Alle Moderatoren
                var moderatoren = await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Moderator")))
                    .Where(u => u.Id != userId) // Nicht sich selbst
                    .OrderBy(u => u.UserName)
                    .Select(u => new ReceiverSelectItem
                    {
                        Type = "Moderator",
                        UserId = u.Id,
                        DisplayName = "🛡️ " + (u.UserName ?? "Moderator").Replace("@suendenbock.lore", "")
                    })
                    .ToListAsync();

                availableReceivers.AddRange(moderatoren);
            }
            else
            {
                // Spieler: Können an alle Charaktere außer sich selbst + Gott + Moderatoren schreiben
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // 1. Andere Charaktere
                var characters = await _context.Characters
                    .Where(c => c.Id != userCharacter.Id && c.UserId != null && c.UserId != "")
                    .OrderBy(c => c.Vorname)
                    .Select(c => new ReceiverSelectItem
                    {
                        Type = "Character",
                        CharacterId = c.Id,
                        DisplayName = c.Vorname
                    })
                    .ToListAsync();

                availableReceivers.AddRange(characters);

                // 2. Alle Götter
                var goetter = await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Gott")))
                    .OrderBy(u => u.UserName)
                    .Select(u => new ReceiverSelectItem
                    {
                        Type = "Gott",
                        UserId = u.Id,
                        DisplayName = "⚡ " + (u.UserName ?? "Gott").Replace("@suendenbock.lore", "")
                    })
                    .ToListAsync();

                availableReceivers.AddRange(goetter);

                // 3. Alle Moderatoren
                var moderatoren = await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Moderator")))
                    .OrderBy(u => u.UserName)
                    .Select(u => new ReceiverSelectItem
                    {
                        Type = "Moderator",
                        UserId = u.Id,
                        DisplayName = "🛡️ " + (u.UserName ?? "Moderator").Replace("@suendenbock.lore", "")
                    })
                    .ToListAsync();

                availableReceivers.AddRange(moderatoren);
            }

            var viewModel = new ComposeMessageViewModel
            {
                AvailableReceivers = availableReceivers,
                CanSendGroupMessage = isGott || isModerator // Nur Gott/Moderator können Gruppennachrichten senden
            };

            // Wenn Antwort auf eine Nachricht
            if (replyTo.HasValue)
            {
                var originalMessage = await _context.Messages
                    .Include(m => m.SenderCharacter)
                    .FirstOrDefaultAsync(m => m.Id == replyTo.Value);

                if (originalMessage != null)
                {
                    // Bei Antwort: Bestimme Empfänger basierend auf SenderType der Original-Nachricht
                    if (originalMessage.SenderType == Models.Domain.MessageSenderType.Character && originalMessage.SenderCharacterId.HasValue)
                    {
                        // Antwort an Charakter
                        ViewBag.ReplyToValue = $"char:{originalMessage.SenderCharacterId.Value}";
                    }
                    else if (originalMessage.SenderType == Models.Domain.MessageSenderType.Gott && !string.IsNullOrEmpty(originalMessage.SenderUserId))
                    {
                        // Antwort an Gott
                        ViewBag.ReplyToValue = $"user:{originalMessage.SenderUserId}:Gott";
                    }
                    else if (originalMessage.SenderType == Models.Domain.MessageSenderType.Moderator && !string.IsNullOrEmpty(originalMessage.SenderUserId))
                    {
                        // Antwort an Moderator
                        ViewBag.ReplyToValue = $"user:{originalMessage.SenderUserId}:Moderator";
                    }

                    ViewBag.ReplyToSubject = "Re: " + originalMessage.Subject;
                }
            }

            return View(viewModel);
        }

        // GET: /Messages/Admin - Admin Dashboard
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Admin(MessageFilters? filters)
        {
            var query = _context.Messages
                .Include(m => m.SenderCharacter)
                .Include(m => m.ReceiverCharacter)
                .AsQueryable();

            // Filter anwenden
            if (filters != null)
            {
                if (!string.IsNullOrWhiteSpace(filters.SenderName))
                {
                    query = query.Where(m =>
                        (m.SenderCharacter != null && m.SenderCharacter.Vorname.Contains(filters.SenderName)) ||
                        (m.SenderDisplayName != null && m.SenderDisplayName.Contains(filters.SenderName)));
                }

                if (!string.IsNullOrWhiteSpace(filters.ReceiverName))
                {
                    query = query.Where(m => m.ReceiverCharacter != null && m.ReceiverCharacter.Vorname.Contains(filters.ReceiverName));
                }

                if (!string.IsNullOrWhiteSpace(filters.Subject))
                {
                    query = query.Where(m => m.Subject.Contains(filters.Subject));
                }

                if (filters.IsRead.HasValue)
                {
                    query = query.Where(m => m.IsRead == filters.IsRead.Value);
                }

                if (filters.IsDeleted.HasValue && filters.IsDeleted.Value)
                {
                    query = query.Where(m => m.IsDeletedBySender || m.IsDeletedByReceiver);
                }

                if (filters.DateFrom.HasValue)
                {
                    query = query.Where(m => m.SentAt >= filters.DateFrom.Value);
                }

                if (filters.DateTo.HasValue)
                {
                    query = query.Where(m => m.SentAt <= filters.DateTo.Value);
                }
            }

            var messagesList = await query
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var messages = messagesList.Select(m => new MessageAdminItem
            {
                Id = m.Id,
                SenderCharacterName = m.SenderType == Models.Domain.MessageSenderType.Character && m.SenderCharacter != null
                    ? m.SenderCharacter.Vorname
                    : m.SenderDisplayName ?? (m.SenderType == Models.Domain.MessageSenderType.Gott ? "Gott" : "Moderator"),
                ReceiverCharacterName = m.ReceiverCharacterId.HasValue && m.ReceiverCharacter != null
                    ? m.ReceiverCharacter.Vorname
                    : "System",
                Subject = m.Subject,
                ContentPreview = m.Content.Length > 100 ? m.Content.Substring(0, 100) + "..." : m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead,
                IsDeletedBySender = m.IsDeletedBySender,
                IsDeletedByReceiver = m.IsDeletedByReceiver
            }).ToList();

            var viewModel = new MessagesAdminViewModel
            {
                Messages = messages,
                TotalMessages = messages.Count,
                Filters = filters ?? new MessageFilters()
            };

            return View(viewModel);
        }
    }
}
