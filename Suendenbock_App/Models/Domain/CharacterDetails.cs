namespace Suendenbock_App.Models.Domain
{
    public class CharacterDetails
    {
        public int Id { get; set; }
        public int CharacterId { get; set; } // Foreign key to Character
        public string? quote { get; set; }
        public string? urheber { get; set; }
        public string? Description { get; set; }
        public string? Beruf { get; set; }
        //Körperliche Eigenschaften
        public int? BodyHeight { get; set; } 
        //Herkunft und Status (alles optional)        
        public int? StandId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }
        //Navigation Properties
        public virtual Character? Character { get; set; } = null!; // Navigation property for Character
        public virtual Stand? Stand { get; set; } // character has one Stand
        public virtual Blutgruppe? Blutgruppe { get; set; } // character has one Blutgruppe
        public virtual Haus? Haus { get; set; } // character has one Haus
        public virtual Herkunftsland? Herkunftsland { get; set; } // character has one Herkunftsland        
    }
}
