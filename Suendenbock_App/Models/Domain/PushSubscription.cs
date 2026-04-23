using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Repräsentiert ein Push-Benachrichtigungs-Abonnement für einen Benutzer auf einem Gerät.
    /// Speichert die notwendigen Informationen um Web Push Notifications zu versenden.
    /// </summary>
    public class PushSubscription
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Die ID des Benutzers der dieses Abonnement besitzt
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Der eindeutige Push-Endpoint vom Browser
        /// </summary>
        [Required]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// Der P256DH-Key für die Verschlüsselung
        /// </summary>
        [Required]
        public string P256dh { get; set; } = string.Empty;

        /// <summary>
        /// Der Auth-Key für die Authentifizierung
        /// </summary>
        [Required]
        public string Auth { get; set; } = string.Empty;

        /// <summary>
        /// Zeitpunkt der Abonnierung
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Letzter erfolgreicher Push
        /// </summary>
        public DateTime? LastSuccessfulPush { get; set; }

        /// <summary>
        /// Anzahl fehlgeschlagener Push-Versuche (für automatisches Löschen ungültiger Subscriptions)
        /// </summary>
        public int FailureCount { get; set; } = 0;

        /// <summary>
        /// Ist das Abonnement aktiv?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// User-Agent String zur Identifikation des Geräts/Browsers
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;
    }
}
