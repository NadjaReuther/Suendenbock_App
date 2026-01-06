// Models/Quest.cs
using Suendenbock_App.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models
{
    /// <summary>
    /// Eine Quest im Spiel
    /// </summary>
    public class Quest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Titel ist erforderlich")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Die Beschreibung ist erforderlich")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Quest-Typ: "individual" = einzelner Charakter, "group" = ganze Gruppe
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "group"; // "individual" oder "group"

        /// <summary>
        /// Status: "active" = läuft, "completed" = abgeschlossen, "failed" = gescheitert
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "active"; // "active", "completed", "failed"

        // ===== BEZIEHUNGEN =====

        /// <summary>
        /// Zu welchem Act gehört diese Quest? (Required)
        /// </summary>
        [Required]
        public int ActId { get; set; }
        public Act Act { get; set; } = null!;

        /// <summary>
        /// Nur gefüllt wenn Type = "individual"
        /// NULL wenn Type = "group" (gilt dann für alle Spieler)
        /// </summary>
        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        /// <summary>
        /// Optional: Auf welchem Kartenmarker ist diese Quest?
        /// </summary>
        public int? MapMarkerId { get; set; }
        public MapMarker? MapMarker { get; set; }

        /// <summary>
        /// Optional: Vorgänger-Quest für Questfolgen
        /// Wenn gesetzt, wird diese Quest nur angezeigt, wenn die Vorgänger-Quest die Bedingung erfüllt
        /// </summary>
        public int? PreviousQuestId { get; set; }
        public Quest? PreviousQuest { get; set; }

        /// <summary>
        /// Bedingung, unter der diese Folgequest erscheint
        /// "completed" = nur bei erfolgreichem Abschluss
        /// "failed" = nur bei Abbruch/Fehlschlag
        /// "both" = bei beidem (Standard)
        /// </summary>
        [StringLength(50)]
        public string? PreviousQuestRequirement { get; set; } = "both"; // "completed", "failed", "both"

        /// <summary>
        /// Nachfolgende Quests (umgekehrte Navigation)
        /// </summary>
        public List<Quest> FollowingQuests { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
    }
}