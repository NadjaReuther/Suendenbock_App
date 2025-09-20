namespace Suendenbock_App.Models.Domain
{
    public class CharacterDetails
    {
        public int Id { get; set; }
        public int CharacterId { get; set; } // Foreign key to Character
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        //Körperliche Eigenschaften
        public int? BodyHeight { get; set; } 
        //Herkunft und Status (alles optional)        
        public int? StandId { get; set; }
        public int? BerufId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }
        //Navigation Properties
        public virtual Character? Character { get; set; } = null!; // Navigation property for Character
        public virtual Stand? Stand { get; set; } // character has one Stand
        public virtual Beruf? Beruf { get; set; } // character has one Beruf
        public virtual Blutgruppe? Blutgruppe { get; set; } // character has one Blutgruppe
        public virtual Haus? Haus { get; set; } // character has one Haus
        public virtual Herkunftsland? Herkunftsland { get; set; } // character has one Herkunftsland        
    }
}
