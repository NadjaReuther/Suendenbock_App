namespace Suendenbock_App.Models.Domain
{
    public class Religion
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;

        //Navigation Properties
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Religion has many Characters

    }
}
