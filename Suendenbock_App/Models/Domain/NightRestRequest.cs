using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Repräsentiert eine offene Nachtlager-Anfrage von einem Spieler.
    /// Wird in DB gespeichert damit der Gott sie auch sieht wenn er später zum Dashboard kommt.
    /// </summary>
    public class NightRestRequest
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Act für den die Anfrage gilt
        /// </summary>
        public int ActId { get; set; }

        /// <summary>
        /// Name des Spielers der die Anfrage gestellt hat
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        /// Zeitpunkt der Anfrage
        /// </summary>
        public DateTime RequestedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Ist die Anfrage noch aktiv/offen?
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
