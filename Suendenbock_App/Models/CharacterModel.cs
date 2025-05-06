namespace Suendenbock_App.Models
{
    public class CharacterModel
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
        public virtual MagicClassModel MagicClass { get; set; } //character has one magic class
        public virtual GuildModel Guild { get; set; } //character has one guild
        public virtual ReligionModel Religion { get; set; } //character has one religion

    }
}
