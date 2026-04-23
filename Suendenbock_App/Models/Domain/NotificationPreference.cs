using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Repräsentiert die Benachrichtigungs-Einstellungen eines Benutzers.
    /// Bestimmt welche Art von Push-Benachrichtigungen der Benutzer erhalten möchte.
    /// </summary>
    public class NotificationPreference
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Die ID des Benutzers
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Benachrichtigungen für neue Forumsbeiträge
        /// </summary>
        public bool NotifyForumThreads { get; set; } = true;

        /// <summary>
        /// Benachrichtigungen für neue Forum-Kommentare
        /// </summary>
        public bool NotifyForumReplies { get; set; } = true;

        /// <summary>
        /// Benachrichtigungen für neue News-Artikel
        /// </summary>
        public bool NotifyNews { get; set; } = true;

        /// <summary>
        /// Benachrichtigungen für neue Umfragen
        /// </summary>
        public bool NotifyPolls { get; set; } = true;

        /// <summary>
        /// Zeitpunkt der Erstellung
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Letztes Update der Einstellungen
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
