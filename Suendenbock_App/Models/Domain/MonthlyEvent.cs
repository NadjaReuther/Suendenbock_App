using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class MonthlyEvent
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Der Titel ist erforderlich")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } =string.Empty;
        [Required]
        public DateTime Date {  get; set; }
        [Required]
        [StringLength(10)]
        public string StartTime { get; set; } = string.Empty;
        [Required]
        [StringLength(10)]
        public string EndTime { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty; // enum "Spieltag", "Vorbereitung", "Ausflug"
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }

        //Navigation Properties
        public ApplicationUser CreatedBy {  get; set; } = null!;
        public List<EventChore> Chores { get; set; } = new List<EventChore>();
        public List<EventRSVP> RSVPs { get; set; } = new List<EventRSVP>();

        // Computed Properties
        public int ParticipantsCount => RSVPs?.Count(r => r.Status == "yes") ?? 0;
    }
}
