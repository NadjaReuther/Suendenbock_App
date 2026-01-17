namespace Suendenbock_App.Models.ViewModels
{
    public class NewsPageViewModel
    {
        public List<NewsItemViewModel> NewsItems { get; set; } = new();
        public bool IsAdmin { get; set; }
    }

    public class NewsItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool IsArchived { get; set; }
        public bool CanEdit { get; set; }
        public List<NewsCommentViewModel> Comments { get; set; } = new();
    }

    public class NewsCommentViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public bool CanDelete { get; set; }
    }
}
