using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class CharacterAffiliation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; } // Foreign key to Character
        //entweder Gilde ODER Infanterie (nicht beides)
        [Display(Name = "Gilde")]
        public int? GuildId { get; set; }
        [Display(Name = "Regiment")]
        public int? RegimentId { get; set; } //Foreign Key für Regiment
        [Display(Name = "Infanterierang")]
        public int? InfanterierangId { get; set; } //Foreign Key für Infanterierang
        //Religion (optional)
        [Display(Name = "Religion")]
        public int? ReligionId { get; set; }
        //Navigation Properties
        public virtual Character? Character { get; set; } = null!; //character has one affiliation
        public virtual Guild? Guild { get; set; } //character has one guild
        public virtual Regiment? Regiment { get; set; } //character has one Regiment
        public virtual Infanterierang? Infanterierang { get; set; } //character has one infanterierang
        public virtual Religion? Religion { get; set; } //character has one religion        
    }
}
