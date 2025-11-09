namespace Suendenbock_App.Models.Domain
{
    public enum TriggerPreferenceLevel
    {
        Okay,
        Vorsichtig,
        Stop
    }

    public class UserTriggerPreference
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TopicId { get; set; }
        public TriggerPreferenceLevel? Preference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; }
        public TriggerTopic Topic { get; set; }
    }
}