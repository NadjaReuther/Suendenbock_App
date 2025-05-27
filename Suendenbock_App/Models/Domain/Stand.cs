namespace Suendenbock_App.Models.Domain
{
    public class Stand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Navigation Property für Charaktere (1:n Beziehung)
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
