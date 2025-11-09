using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class CharacterDetails
    {
        public int Id { get; set; }
        public int CharacterId { get; set; } // Foreign key to Character
        [Display(Name = "Zitat")]
        public string? quote { get; set; }
        [Display(Name = "Urheber des Zitats")]
        public string? urheber { get; set; }
        [Display(Name = "Beschreibung")]
        public string? Description { get; set; }
        [Display(Name = "Beruf")]
        [StringLength(200)]
        public string? Beruf { get; set; }
        //Körperliche Eigenschaften
        [Display(Name = "Körpergröße (cm)")]
        public int? BodyHeight { get; set; }
        //Herkunft und Status (alles optional)        
        [Display(Name = "Stand")]
        public int? StandId { get; set; }
        [Display(Name = "Blutgruppe")]
        public int? BlutgruppeId { get; set; }
        [Display(Name = "Haus")]
        public int? HausId { get; set; }
        [Display(Name = "Herkunftsland")]
        public int? HerkunftslandId { get; set; }
        //Navigation Properties
        public virtual Character? Character { get; set; } = null!; // Navigation property for Character
        public virtual Stand? Stand { get; set; } // character has one Stand
        public virtual Blutgruppe? Blutgruppe { get; set; } // character has one Blutgruppe
        public virtual Haus? Haus { get; set; } // character has one Haus
        public virtual Herkunftsland? Herkunftsland { get; set; } // character has one Herkunftsland        
    }
}
