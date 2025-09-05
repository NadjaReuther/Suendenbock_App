namespace Suendenbock_App.Models.Domain
{
    public class Regiment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Foreign Key zu Characters
        public int Regimentsleiter { get; set; } //character id for leader
        public int Adjutant { get; set; } //character id for adjutant
        public int Infanterie { get; set; } // one Infanterie has many Regiments
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Regiments has many Characters

        // Navigation Properties hinzufügen
        public virtual Character? RegimentsCharacter { get; set; } // Für den Leader
        public virtual Character? AdjutantCharacter { get; set; } // Für den Stellvertreter
    }
}
