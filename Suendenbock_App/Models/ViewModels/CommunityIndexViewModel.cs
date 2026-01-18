namespace Suendenbock_App.Models.ViewModels
{
    public class CommunityIndexViewModel
    {
        public List<NewsPreview> RecentNews { get; set; } = new List<NewsPreview>();
        public List<EventViewModel> UpcomingEvents { get; set; } = new List<EventViewModel>();
        public List<ForumThreadPreview> RecentThreads { get; set; } = new List<ForumThreadPreview>();
        public List<PollPreview> ActivePolls { get; set; } = new List<PollPreview>();
        public List<MonthlyPaymentViewModel> MonthlyPayments { get; set; } = new List<MonthlyPaymentViewModel>();

        public int TotalEvents { get; set; }
        public int TotalThreads { get; set; }
        public int TotalPolls { get; set; }
        public bool IsAdmin { get; set; }
    }

    // Preview Models für die Übersicht
    public class NewsPreview
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class ForumThreadPreview
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ReplyCount { get; set; }
        public bool IsPinned { get; set; }
    }

    public class PollPreview
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
