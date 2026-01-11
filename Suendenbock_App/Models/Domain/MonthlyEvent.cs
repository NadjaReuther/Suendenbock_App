namespace Suendenbock_App.Models.Domain
{
    public class MonthlyEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly Date {  get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Type { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        //Navigation Properties
        public ApplicationUser CreatedBy {  get; set; }
        public List<EventChore> Chores { get; set; }
        public List<EventRSVP> RSVPs { get; set; }
    }
}
