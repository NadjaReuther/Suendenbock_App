namespace Suendenbock_App.Models.Domain
{
    public class Beruf
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //Navigation Property für Charaktere
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Beruf has many characters
    }
}
