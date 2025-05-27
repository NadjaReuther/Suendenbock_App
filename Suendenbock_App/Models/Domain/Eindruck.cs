namespace Suendenbock_App.Models.Domain
{
    public class Eindruck
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //Navigation Property für Charaktere (1:n)
        public virtual ICollection<Character> Charaktere { get; set; } = new List<Character>(); // Eindruck has many characters
    }
}
