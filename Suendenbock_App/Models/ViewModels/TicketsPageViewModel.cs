namespace Suendenbock_App.Models.ViewModels
{
    public class TicketsPageViewModel
    {
        public List<TicketViewModel> Tickets { get; set; } = new();
        public bool IsAdmin { get; set; }
    }

    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string? ResolvedAt { get; set; }
        public string? ResolvedByName { get; set; }
        public bool CanEdit { get; set; }
    }
}
