namespace Suendenbock_App.Models.Domain
{
    public class Lebensstatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //Navigation property to Character (1:n)
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
