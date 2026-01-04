using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models
{
    /// <summary>
    /// Eine aktive Kampf-Session.
    /// Wenn Gott einen Kampf startet, wird eine CombatSession erstellt.
    /// Spieler können sich mit dieser Session verbinden und sehen den Kampf live via SignalR.
    /// </summary>
    public class CombatSession
    {
        public int Id { get; set; }

        /// <summary>
        /// Zu welchem Akt gehört dieser Kampf?
        /// </summary>
        public int ActId { get; set; }
        public Act Act { get; set; } = null!;

        /// <summary>
        /// Ist dieser Kampf gerade aktiv? (nur ein aktiver Kampf pro Akt erlaubt)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Aktuelle Runde des Kampfes
        /// </summary>
        public int CurrentRound { get; set; } = 1;

        /// <summary>
        /// Index des aktuellen Teilnehmers (wer ist gerade am Zug?)
        /// </summary>
        public int CurrentTurnIndex { get; set; } = 0;

        /// <summary>
        /// JSON-String mit dem kompletten Battle State
        /// Format: { participants: [...], expandedConditions: {...} }
        ///
        /// Dieser enthält:
        /// - Alle Teilnehmer (Spieler, Begleiter, Monster)
        /// - Initiative-Werte
        /// - HP, Pokus, Temp HP
        /// - Aktive Conditions mit Levels/Counters
        /// - Death Saves Status
        /// - etc.
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string BattleStateJson { get; set; } = "{}";

        /// <summary>
        /// Wann wurde dieser Kampf gestartet?
        /// </summary>
        public DateTime StartedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Wann wurde der Kampf beendet? (null = noch aktiv)
        /// </summary>
        public DateTime? EndedAt { get; set; }

        /// <summary>
        /// Ergebnis des Kampfes: "victory", "defeat", null (noch nicht entschieden)
        /// </summary>
        [StringLength(50)]
        public string? Result { get; set; }
    }
}
