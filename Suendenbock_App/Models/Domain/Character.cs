using System.ComponentModel.DataAnnotations;

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
      
        [Display(Name = "Nachname")]
        public string Nachname { get; set; } = string.Empty;

        [Display(Name = "Vorname")]
        public string Vorname { get; set; } = string.Empty;

        [Display(Name = "Rufname")]
        public string Rufname { get; set; } = string.Empty;

        [Display(Name = "Geschlecht")]
        public string Geschlecht { get; set; } = string.Empty;
        public string? UserId { get; set; } //optionaler Foreign-Key zu User
        public string? UserColor { get; set; } //optionale benutzerdefinierte Farbe für den Charakter   
        [Display(Name = "Profan")]
        public bool Profan { get; set; } = false;
        [Display(Name = "Beschränkt")]
        public bool Beschraenkt { get; set; } = false;
        public bool IsUnbegabt { get; set; } = false;

        [Display(Name = "Begleitcharakter")]
        public bool IsCompanion { get; set; } = false;

        //Pflicht-Foreign-Keys
        [Display(Name = "Rasse")]
        public int RasseId { get; set; }

        [Display(Name = "Lebensstatus")]
        public int LebensstatusId { get; set; }

        [Display(Name = "Eindruck")]
        public int EindruckId { get; set; }
        //für Spielmodus-System
        // ===== STAMMDATEN =====

        /// <summary>
        /// Basis-Maximalwert für Lebenspunkte (aus Charakterbogen)
        /// </summary>
        public int BaseMaxHealth { get; set; } = 150;

        /// <summary>
        /// Basis-Maximalwert für Pokuspunkte (aus Charakterbogen)
        /// </summary>
        public int BaseMaxPokus { get; set; } = 30;

        // ===== AKTUELLER SPIELZUSTAND (wird während des Spiels geändert) =====

        /// <summary>
        /// Aktuelle Lebenspunkte - wird im Spiel angepasst
        /// </summary>
        public int CurrentHealth { get; set; } = 50;

        /// <summary>
        /// Aktuelle Pokuspunkte - wird im Spiel angepasst
        /// </summary>
        public int CurrentPokus { get; set; } = 0;

        /// <summary>
        /// Wann wurde zuletzt gerastet? (für Nachtlager-Tracking)
        /// </summary>
        public DateTime? LastRestAt { get; set; }

        // ===== BEZIEHUNGEN =====

        /// <summary>
        /// Individual-Quests, die diesem Charakter zugewiesen sind
        /// </summary>
        public List<Quest> IndividualQuests { get; set; } = new();

        //Optionale Basis-Informationen
        [Display(Name = "Geburtsdatum")]
        public string? Geburtsdatum { get; set; }
        public string? ImagePath { get; set; }

        //Eltern-Beziehungen (optional)
        [Display(Name = "Vater")]
        public int? VaterId { get; set; }
        [Display(Name = "Mutter")]
        public int? MutterId { get; set; }
        //Partner-Beziehung (optional, wechselseitig)
        [Display(Name = "Partner")]
        public int? PartnerId { get; set; } //optional Foreign-Key zu Partner (Character)

        //Vollständigkeits-Status
        public CharacterCompleteness CompletionLevel { get; set; } = CharacterCompleteness.BasicInfo;
        
        //Navigation Properties für Pflichtfelder
        public virtual Rasse Rasse { get; set; } = null!; //character has one Rasse
        public virtual Lebensstatus Lebensstatus { get; set; } = null!; //character has one Lebensstatus
        public virtual Eindruck Eindruck { get; set; } = null!; //character has one Eindruck
        //Navigation Properties für Eltern
        public virtual Character? Vater { get; set; } //character has one father
        public virtual Character? Mutter { get; set; } //character has one mother
        public virtual Character? Partner { get; set; } //character has one partner
        //Navigation Properties für andere Tabellen
        public virtual CharacterDetails? Details { get; set; }
        public virtual CharacterAffiliation? Affiliation { get; set; }
        //Magie-Beziehungen (Pflicht - mindestens eine Magie-Klasse)
        public virtual ICollection<CharacterMagicClass> CharacterMagicClasses { get; set; } = new List<CharacterMagicClass>();
    }
}
