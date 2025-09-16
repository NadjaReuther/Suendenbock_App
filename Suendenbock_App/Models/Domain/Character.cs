namespace Suendenbock_App.Models.Domain
{
    public enum CharacterCompleteness
    {
        BasicInfo = 1, // nur Pflichtfelder
        WithDetails = 2, // + persönliche Detals
        Complete = 3 // + Zugehörigkeiten
    }
    public class Character
    {
        public int Id { get; set; }
        public string Nachname { get; set; } = string.Empty;
        public string Vorname { get; set; } = string.Empty;
        public string Rufname { get; set; } = string.Empty;
        public string Geschlecht { get; set; } = string.Empty;

        //Pflicht-Foreign-Keys
        public int RasseId { get; set; }
        public int LebensstatusId { get; set; }
        public int EindruckId { get; set; }

        //Optionale Basis-Informationen
        public string? Geburtsdatum { get; set; }
        public string? ImagePath { get; set; }
        //Eltern-Bjeziehungen (optional)
        public int? VaterId { get; set; }
        public int? MutterId { get; set; }
        //Vollständigkeits-Status
        public CharacterCompleteness CompletionLevel { get; set; } = CharacterCompleteness.BasicInfo;
        //Navigation Properties für Pflichtfelder
        public virtual Rasse Rasse { get; set; } = null!; //character has one Rasse
        public virtual Lebensstatus Lebensstatus { get; set; } = null!; //character has one Lebensstatus
        public virtual Eindruck Eindruck { get; set; } = null!; //character has one Eindruck
        //Navigation Properties für Eltern
        public virtual Character? Vater { get; set; } //character has one father
        public virtual Character? Mutter { get; set; } //character has one mother
        //Navigation Properties für andere Tabellen
        public virtual CharacterDetails? Details { get; set; }
        public virtual CharacterAffiliation? Affiliation { get; set; }
        //Magie-Beziehungen (Pflicht - mindestens eine Magie-Klasse)
        public virtual ICollection<CharacterMagicClass> CharacterMagicClasses { get; set; } = new List<CharacterMagicClass>();
    }
}
