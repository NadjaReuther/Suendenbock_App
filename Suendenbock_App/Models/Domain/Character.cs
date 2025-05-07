namespace Suendenbock_App.Models.Domain
{
    public class Character
    {
        public int Id { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public string Geschlecht { get; set; }
        public string? Geburtsdatum { get; set; }
        public string? ImagePath { get; set; }

        //Foreign Keys
        public int? MagicClassId { get; set; }
        public int? GuildId { get; set; }
        public int? ReligionId { get; set; }

        //Navigation Properties
        public virtual MagicClass? MagicClass { get; set; } //character has one magic class
        public virtual Guild? Guild { get; set; } //character has one guild
        public virtual Religion? Religion { get; set; } //character has one religion

    }
}
