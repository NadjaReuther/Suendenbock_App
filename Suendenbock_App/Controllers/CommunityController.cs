using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.ViewModels;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class CommunityController : Controller
    {
        private ApplicationDbContext _context;

        public CommunityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==== INDEX / OVERVIEW ====
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            // Lade die nächsten 5 Events
            var upcomingEvents = await _context.MonthlyEvents
                .Include(e => e.RSVPs)
                .Include(e => e.Chores)
                .Where(e => e.Date >= DateTime.Today)
                .OrderBy(e => e.Date)
                .Take(5)
                .ToListAsync();

            // Konvertiere zu ViewModels
            var eventViewModels = upcomingEvents.Select(e => {
                var userRsvp = e.RSVPs.FirstOrDefault(r => r.UserId == userId);

                Dictionary<string, string>? chores = null;
                if (e.Type == "Spieltag")
                {
                    if (e.Chores.Any())
                    {
                        chores = e.Chores.ToDictionary(
                            c => c.ChoreName,
                            c => c.AssignedToName ?? "Offen"
                        );
                    }
                    else
                    {
                        chores = new Dictionary<string, string>();
                    }
                }

                var months = new[] { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

                return new EventViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Date = e.Date,
                    DisplayDate = e.Date.ToString("dd.MM.yyyy"),
                    Day = e.Date.Day.ToString("D2"),
                    Month = months[e.Date.Month - 1],
                    Type = e.Type,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Description = e.Description,
                    ParticipantsCount = e.ParticipantsCount,
                    CurrentUserRSVP = userRsvp?.Status,
                    Chores = chores,
                    HasChores = e.Type == "Spieltag",
                    CanEdit = e.CreatedByUserId == userId || User.IsInRole("Gott"),
                    CreatedByUserId = e.CreatedByUserId
                };
            }).ToList();

            // Lade die neuesten 2 News (nur die jünger als 1 Monat sind)
            var oneMonthAgo = DateTime.Now.AddMonths(-1);
            var recentNews = await _context.NewsItems
                .Where(n => n.CreatedAt >= oneMonthAgo)
                .OrderByDescending(n => n.CreatedAt)
                .Take(2)
                .Select(n => new NewsPreview
                {
                    Id = n.Id,
                    Title = n.Title,
                    Excerpt = n.Excerpt,
                    Icon = n.Icon,
                    Author = n.Author,
                    Date = n.CreatedAt.ToString("dd.MM.yyyy")
                })
                .ToListAsync();

            // Lade die neuesten 5 Forum-Threads
            var recentThreads = await _context.ForumThreads
                .Include(t => t.Category)
                .Include(t => t.Replies)
                .Include(t => t.AuthorCharacter)
                .Include(t => t.AuthorUser)
                .Where(t => !t.IsArchived)
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new ForumThreadPreview
                {
                    Id = t.Id,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    AuthorName = t.AuthorCharacter != null ? t.AuthorCharacter.Vorname : (t.AuthorUser != null ? t.AuthorUser.UserName : "Unbekannt"),
                    CreatedAt = t.CreatedAt,
                    ReplyCount = t.Replies.Count,
                    IsPinned = t.IsPinned
                })
                .ToListAsync();

            // Lade die aktuellen 5 Polls
            var activePolls = await _context.Polls
                .Include(p => p.Votes)
                .Where(p => p.Status == "active")
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new PollPreview
                {
                    Id = p.Id,
                    Question = p.Question,
                    Category = p.Category,
                    TotalVotes = p.Votes.Select(v => v.CharacterId ?? v.UserId.GetHashCode()).Distinct().Count(),
                    CreatedAt = p.CreatedAt,
                    Status = p.Status
                })
                .ToListAsync();

            var viewModel = new CommunityIndexViewModel
            {
                RecentNews = recentNews,
                UpcomingEvents = eventViewModels,
                RecentThreads = recentThreads,
                ActivePolls = activePolls,
                TotalEvents = await _context.MonthlyEvents.CountAsync(),
                TotalThreads = await _context.ForumThreads.Where(t => !t.IsArchived).CountAsync(),
                TotalPolls = await _context.Polls.Where(p => p.Status == "active").CountAsync()
            };

            return View(viewModel);
        }

        // ==== EVENTS ====
        public async Task<IActionResult> Events()
        {
            // TODO: Implementierung folgt
            var userId = GetUserId();

            // Lade alle Events mit RSVPs und Chores
            var events = await _context.MonthlyEvents
                .Include(e => e.RSVPs)
                .Include(e => e.Chores)
                .Include(e => e.CreatedBy)
                .OrderBy(e => e.Date)
                .ToListAsync();

            // Konvertiere zu ViewModels
            var eventViewModels = events.Select(e =>
            {
                // Finde RSVP des aktuellen Users
                var userRsvp = e.RSVPs.FirstOrDefault(r => r.UserId == userId);

                // Erstelle Chore-Dictionary ( nur bei Spieltagen )
                Dictionary<string, string>? chores = null;
                if (e.Type == "Spieltag" && e.Chores.Any())
                {
                    chores = e.Chores.ToDictionary(
                        c => c.ChoreName,
                        c => c.AssignedToName ?? "Offen"
                    );
                }

                // Formatiere Daten
                var months = new[] { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };

                return new EventViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Date = e.Date,
                    DisplayDate = e.Date.ToString("dd.MM.yyyy"),
                    Day = e.Date.Day.ToString("D2"),
                    Month = months[e.Date.Month - 1],
                    Type = e.Type,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Description = e.Description,
                    ParticipantsCount = e.ParticipantsCount,
                    CurrentUserRSVP = userRsvp?.Status,
                    Chores = chores,
                    HasChores = chores?.Any() ?? false,
                    CanEdit = e.CreatedByUserId == userId || User.IsInRole("Gott"),
                    CreatedByUserId = e.CreatedByUserId
                };
            }).ToList();

            var viewModel = new EventsPageViewModel
            {
                Events = eventViewModels,
                IsAdmin = User.IsInRole("Gott")
            };

            return View(viewModel);
        }

        // ==== FORUM ====
        public async Task<IActionResult> Forum()
        {
            // TODO: Implementierung folgt
            return View();
        }

        // ==== POLLS ====
        public async Task<IActionResult> Polls()
        {
            var userId = GetUserId();

            // Lade alle Polls mit Options und Votes
            var polls = await _context.Polls
                .Include(p => p.Options)
                    .ThenInclude(o => o.Votes)
                        .ThenInclude(v => v.User)
                .Include(p => p.Options)
                    .ThenInclude(o => o.Votes)
                        .ThenInclude(v => v.Character)
                .Include(p => p.Votes)
                    .ThenInclude(v => v.User)
                .Include(p => p.Votes)
                    .ThenInclude(v => v.Character)
                .OrderByDescending(p => p.Status == "active")
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            // Konvertiere zu ViewModels
            var pollViewModels = polls.Select(p =>
            {
                // Finde die Optionen, für die der User gestimmt hat
                var userVotedOptionIds = p.Votes
                    .Where(v => v.UserId == userId)
                    .Select(v => v.PollOptionId)
                    .ToList();

                // Berechne Prozentsätze für jede Option
                var totalVoters = p.TotalVoters;
                var options = p.Options.OrderBy(o => o.SortOrder).Select(o =>
                {
                    var voteCount = o.Votes.Count;
                    var percentage = totalVoters > 0 ? (double)voteCount / totalVoters * 100 : 0;

                    return new PollOptionViewModel
                    {
                        Id = o.Id,
                        Text = o.Text,
                        Votes = voteCount,
                        Percentage = percentage
                    };
                }).ToList();

                // Sammle alle Wähler (unique)
                var voterNames = p.Votes
                    .GroupBy(v => v.UserId ?? v.CharacterId?.ToString() ?? "unknown")
                    .Select(g => g.First().VoterName)
                    .OrderBy(name => name)
                    .ToList();

                return new PollViewModel
                {
                    Id = p.Id,
                    Question = p.Question,
                    Status = p.Status,
                    Category = p.Category,
                    AllowMultipleChoices = p.AllowMultipleChoices,
                    TotalVoters = totalVoters,
                    Options = options,
                    UserVotedOptionIds = userVotedOptionIds,
                    VoterNames = voterNames,
                    CanEdit = p.CreatedByUserId == userId || User.IsInRole("Gott")
                };
            }).ToList();

            var viewModel = new PollsPageViewModel
            {
                Polls = pollViewModels,
                IsAdmin = User.IsInRole("Gott")
            };

            return View(viewModel);
        }

        // ==== NEWS ====
        public async Task<IActionResult> News()
        {
            var userId = GetUserId();
            var oneMonthAgo = DateTime.Now.AddMonths(-1);

            // Lade alle News mit Kommentaren
            var newsItems = await _context.NewsItems
                .Include(n => n.Comments)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            // Konvertiere zu ViewModels
            var newsViewModels = newsItems.Select(n =>
            {
                var isArchived = n.CreatedAt < oneMonthAgo;

                return new NewsItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Excerpt = n.Excerpt,
                    Category = n.Category,
                    Icon = n.Icon,
                    Author = n.Author,
                    Date = n.CreatedAt.ToString("dd.MM.yyyy"),
                    IsArchived = isArchived,
                    CanEdit = User.IsInRole("Gott"),
                    Comments = n.Comments.Select(c => new NewsCommentViewModel
                    {
                        Id = c.Id,
                        Text = c.Text,
                        Author = c.Author,
                        Time = c.CreatedAt.ToString("HH:mm") + " Uhr",
                        CanDelete = User.IsInRole("Gott")
                    }).ToList()
                };
            }).ToList();

            var viewModel = new NewsPageViewModel
            {
                NewsItems = newsViewModels,
                IsAdmin = User.IsInRole("Gott")
            };

            return View(viewModel);
        }

        // Helper: Get Current User ID
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
