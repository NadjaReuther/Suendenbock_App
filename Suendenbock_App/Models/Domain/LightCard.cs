namespace Suendenbock_App.Models.Domain
{
    public class LightCard
    {
        public int Id { get; set; }
        public string CssClass { get; set; } = string.Empty;
        public virtual ICollection<Obermagie> Obermagie { get; set; } = new List<Obermagie>(); // LightCard has one or more Obermagie
    }
}
