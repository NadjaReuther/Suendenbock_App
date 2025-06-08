namespace Suendenbock_App.Models.Domain
{
    public class CharacterAffiliation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; } // Foreign key to Character
        //entweder Gilde ODER Infanterie (nicht beides)
        public int? GuildId { get; set; }
        public int? InfanterieId { get; set; } //Foreign Key für Infanterie
        public int? InfanterierangId { get; set; } //Foreign Key für Infanterierang
        //Religion (optional)
        public int? ReligionId { get; set; }
        //Navigation Properties
        public virtual Character? Character { get; set; } = null!; //character has one affiliation
        public virtual Guild? Guild { get; set; } //character has one guild
        public virtual Infanterie? Infanterie { get; set; } //character has one infanterie
        public virtual Infanterierang? Infanterierang { get; set; } //character has one infanterierang
        public virtual Religion? Religion { get; set; } //character has one religion        
    }
}
