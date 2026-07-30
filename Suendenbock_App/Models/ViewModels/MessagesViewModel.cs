namespace Suendenbock_App.Models.ViewModels
{
    public class MessagesInboxViewModel
    {
        public List<MessageListItem> Messages { get; set; } = new();
        public int UnreadCount { get; set; }
    }

    public class MessagesSentViewModel
    {
        public List<MessageListItem> Messages { get; set; } = new();
    }

    public class MessageDetailViewModel
    {
        public int Id { get; set; }
        public string SenderCharacterName { get; set; } = string.Empty;
        public string ReceiverCharacterName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsSender { get; set; } // Ist der aktuelle User der Absender?
        public string SenderType { get; set; } = "Character"; // Character, Gott, Moderator
        public int ThreadId { get; set; }
        public List<ThreadMessageItem> ThreadMessages { get; set; } = new(); // Alle Nachrichten im Thread
    }

    public class ThreadMessageItem
    {
        public int Id { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsSender { get; set; } // Ist der aktuelle User der Absender dieser Nachricht?
        public string SenderType { get; set; } = "Character";
    }

    public class MessageListItem
    {
        public int Id { get; set; }
        public string OtherCharacterName { get; set; } = string.Empty; // Absender (bei Posteingang) oder Empfänger (bei Gesendet)
        public string Subject { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty; // Ersten 100 Zeichen
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }

        // Thread-Informationen
        public int ThreadId { get; set; }
        public int MessageCount { get; set; } = 1; // Anzahl Nachrichten im Thread
        public int UnreadCount { get; set; } = 0; // Anzahl ungelesener Nachrichten im Thread
    }

    public class ComposeMessageViewModel
    {
        public List<ReceiverSelectItem> AvailableReceivers { get; set; } = new();
        public bool CanSendGroupMessage { get; set; } = false; // Nur für Gott/Moderator
    }

    public class ReceiverSelectItem
    {
        public string Type { get; set; } = "Character"; // "Character", "Gott", "Moderator"
        public int? CharacterId { get; set; }
        public string? UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    // Legacy - für Kompatibilität
    public class CharacterSelectItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Admin Dashboard
    public class MessagesAdminViewModel
    {
        public List<MessageAdminItem> Messages { get; set; } = new();
        public int TotalMessages { get; set; }
        public MessageFilters Filters { get; set; } = new();
    }

    public class MessageAdminItem
    {
        public int Id { get; set; }
        public string SenderCharacterName { get; set; } = string.Empty;
        public string ReceiverCharacterName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeletedBySender { get; set; }
        public bool IsDeletedByReceiver { get; set; }
    }

    public class MessageFilters
    {
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public string? Subject { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
