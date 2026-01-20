namespace Suendenbock_App.Models.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        // Spieltag/Vorbereitung/Ausflug
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;

        // RSVP Info
        public int ParticipantsCount { get; set; }
        public string? CurrentUserRSVP { get; set; } // "yes", "maybe", "no" oder null

        // Chores (nur bei Spieltagen
        public Dictionary<string, string>? Chores { get; set; } // ChoreName -> AssignedToName
        public bool HasChores { get; set; }

        // Permissions
        public bool CanEdit { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;

        // Status
        public bool IsPast { get; set; }
    }

    public class EventsPageViewModel
    {
        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();
        public bool IsAdmin { get; set; }
    }
}
