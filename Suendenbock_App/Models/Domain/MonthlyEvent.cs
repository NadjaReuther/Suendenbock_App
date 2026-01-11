using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class MonthlyEvent
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Der Titel ist erforderlich")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } =string.Empty;
        public DateOnly Date {  get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Type { get; set; } = string.Empty; // enum "Spieltag", "Vorbereitung", "Ausflug"
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        //Navigation Properties
        public ApplicationUser? CreatedBy {  get; set; }
        public List<EventChore>? Chores { get; set; }
        public List<EventRSVP>? RSVPs { get; set; }
    }
}
