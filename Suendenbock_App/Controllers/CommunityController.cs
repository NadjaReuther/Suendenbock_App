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
            // TODO: Implementierung folgt
            return View();
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
