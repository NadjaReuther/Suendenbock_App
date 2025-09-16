namespace Suendenbock_App.Models.Domain
{
    public class Monsteranfaelligkeiten
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Monstertypanfaelligkeiten> Monstertypanfaelligkeiten { get; set; } = new List<Monstertypanfaelligkeiten>();
    }
}
