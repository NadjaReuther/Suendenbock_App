namespace Suendenbock_App.Models.Domain
{
    public class Herkunftsland
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
