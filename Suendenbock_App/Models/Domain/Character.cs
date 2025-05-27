namespace Suendenbock_App.Models.Domain
{
    public class Character
    {
        public int Id { get; set; }
        public string Nachname { get; set; } = string.Empty;
        public string Vorname { get; set; } = string.Empty;
        public string Geschlecht { get; set; } = string.Empty;
        public int Bodyheight { get; set; } = 0; // Default value for Bodyheight
        public string? Geburtsdatum { get; set; }
        public string? ImagePath { get; set; }
        //Forein Keys für Rasse, Eindruck, Stand, Beruf, Blutgruppe, Haus, Herkunftsland, Lebensstatus
        public int? RasseId { get; set; }
        public int? EindruckId { get; set; }
        public int? StandId { get; set; }
        public int? BerufId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }
        public int? LebensstatusId { get; set; } //Foreign Key für Lebensstatus

        //Foreign Keys für Eltern (optional)
        public int? VaterId { get; set; }
        public int? MutterId { get; set; }

        //Foreign Keys für Guild und Religion
        public int? GuildId { get; set; }
        public int? ReligionId { get; set; }

        //Navigation Properties für Eltern
        public virtual Character? Vater { get; set; } //character has one father
        public virtual Character? Mutter { get; set; } //character has one mother

        //Navigation Properties für Guild und Religion
        public virtual Guild? Guild { get; set; } //character has one guild
        public virtual Religion? Religion { get; set; } //character has one religion

        //Navigation Property für MagicClasses (1:n)
        public virtual ICollection<MagicClass> MagicClasses { get; set; } = new List<MagicClass>(); //character has many magic classes
    }
}
