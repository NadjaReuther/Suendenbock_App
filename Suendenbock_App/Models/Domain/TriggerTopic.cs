namespace Suendenbock_App.Models.Domain
{
    public class TriggerTopic
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TriggerCategory Category { get; set; }
        public ICollection<UserTriggerPreference> UserPreferences { get; set; } = new List<UserTriggerPreference>();
    }
}