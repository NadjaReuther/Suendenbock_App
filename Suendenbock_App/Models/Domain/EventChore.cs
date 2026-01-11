namespace Suendenbock_App.Models.Domain
{
    public class EventChore
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string ChoreName { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
        public bool IsSpecial { get; set; }

        //Navigation Properties
        public MonthlyEvent? Event { get; set; }
    }
}
