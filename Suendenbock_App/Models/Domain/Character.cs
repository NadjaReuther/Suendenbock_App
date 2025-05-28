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

        //Foreign Keys für Guild und oder Infanterie und Infanterierang und Religion
        public int? GuildId { get; set; }
        public int? InfanterieId { get; set; } //Foreign Key für Infanterie
        public int? InfanterierangId { get; set; } //Foreign Key für Infanterierang
        public int? ReligionId { get; set; }

        //Navigation Properties für Eltern
        public virtual Character? Vater { get; set; } //character has one father
        public virtual Character? Mutter { get; set; } //character has one mother

        //Navigation Properties für Guild/Infanterie und Religion und Infanterierang
        public virtual Guild? Guild { get; set; } //character has one guild
        public virtual Infanterie? Infanterie { get; set; } //character has one infanterie
        public virtual Infanterierang? Infanterierang { get; set; } //character has one infanterierang
        public virtual Religion? Religion { get; set; } //character has one religion
        //Character has many MagicClasses through CharacterMagicClass
        public virtual ICollection<CharacterMagicClass> CharacterMagicClasses { get; set; } = new List<CharacterMagicClass>();
    }
}
