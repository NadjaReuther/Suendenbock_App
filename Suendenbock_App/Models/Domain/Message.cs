using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Interne Nachricht zwischen Charakteren oder von System (Gott/Moderator)
    /// </summary>
    public class Message
    {
        public int Id { get; set; }

        /// <summary>
        /// Typ des Absenders (Character, Gott, Moderator)
        /// </summary>
        [Required]
        public MessageSenderType SenderType { get; set; } = MessageSenderType.Character;

        /// <summary>
        /// Absender-Charakter (nur wenn SenderType = Character)
        /// </summary>
        public int? SenderCharacterId { get; set; }
        public Character? SenderCharacter { get; set; }

        /// <summary>
        /// Absender-User ID (nur wenn SenderType = Gott oder Moderator)
        /// </summary>
        public string? SenderUserId { get; set; }

        /// <summary>
        /// Anzeigename des Absenders (für Gott/Moderator)
        /// </summary>
        public string? SenderDisplayName { get; set; }

        /// <summary>
        /// Empfänger-Charakter (wenn Empfänger = Spieler)
        /// </summary>
        public int? ReceiverCharacterId { get; set; }
        public Character? ReceiverCharacter { get; set; }

        /// <summary>
        /// Empfänger-User ID (wenn Empfänger = Gott oder Moderator)
        /// </summary>
        public string? ReceiverUserId { get; set; }

        /// <summary>
        /// Empfänger-Typ (für Anzeige)
        /// </summary>
        public MessageSenderType? ReceiverType { get; set; }

        /// <summary>
        /// Betreff der Nachricht
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Nachrichteninhalt
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Wann wurde die Nachricht gesendet?
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Wurde die Nachricht vom Empfänger gelesen?
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Wurde vom Absender gelöscht? (für "Gesendet" Ordner)
        /// </summary>
        public bool IsDeletedBySender { get; set; } = false;

        /// <summary>
        /// Wurde vom Empfänger gelöscht? (für "Posteingang" Ordner)
        /// </summary>
        public bool IsDeletedByReceiver { get; set; } = false;

        /// <summary>
        /// Thread-ID: Gruppiert zusammengehörige Nachrichten
        /// Bei neuer Konversation = eigene Message-ID
        /// Bei Antwort = Thread-ID der Original-Nachricht
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Parent Message ID: Verweis auf die Nachricht, auf die geantwortet wurde
        /// Null = Erste Nachricht im Thread
        /// </summary>
        public int? ParentMessageId { get; set; }
    }
}
