namespace Suendenbock_App.Models.Domain
{
    public class TriggerCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TriggerTopic> Topics { get; set; } = new List<TriggerTopic>();
    }
}