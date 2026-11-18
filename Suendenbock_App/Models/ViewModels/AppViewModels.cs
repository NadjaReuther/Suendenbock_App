namespace Suendenbock_App.Models.ViewModels
{
    // ==== Handy-App (/App) ====

    /// <summary>Ein Termin, wie ihn die App zeigt</summary>
    public class AppEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Month { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public string When { get; set; } = string.Empty;          // "Sa, 28.11. · 14:00–22:00"
        public string? Description { get; set; }
        public int YesCount { get; set; }
        public string? MyRsvp { get; set; }                       // yes / maybe / no / null
        public string? MyChore { get; set; }                      // eigener Dienst, falls zuzuordnen
        public List<AppChoreViewModel> Chores { get; set; } = new();
    }

    public class AppChoreViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = "Offen";
        public bool IsMine { get; set; }
    }

    /// <summary>Monatsbeitrag des angemeldeten Spielers</summary>
    public class AppPaymentViewModel
    {
        public string MonthName { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaidAt { get; set; }
    }

    /// <summary>Ein Eintrag für "Was ist neu" und das Pop-up</summary>
    public class AppFeedItem
    {
        public string Kind { get; set; } = string.Empty;          // event, msg, news, poll, forum, pay
        public string Label { get; set; } = string.Empty;         // "Neuer Termin", "Nachricht", …
        public string Icon { get; set; } = string.Empty;          // Material-Symbol
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Timestamp { get; set; }                       // Unix-Millisekunden
    }

    public class AppStartViewModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public AppEventViewModel? NextEvent { get; set; }
        public AppPaymentViewModel? Payment { get; set; }
        public List<AppFeedItem> Feed { get; set; } = new();
    }

    public class AppTermineViewModel
    {
        public List<AppEventViewModel> Events { get; set; } = new();
    }

    public class AppForumViewModel
    {
        public List<ForumCategoryViewModel> Categories { get; set; } = new();
        public string? ActiveCategory { get; set; }
        public List<AppThreadItem> Threads { get; set; } = new();
    }

    public class AppThreadItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int ReplyCount { get; set; }
        public bool IsPinned { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class AppThreadViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = string.Empty;       // HTML wie auf der Website
        public List<AppReplyItem> Replies { get; set; } = new();
    }

    public class AppReplyItem
    {
        public string AuthorName { get; set; } = string.Empty;
        public bool IsMine { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = string.Empty;       // HTML wie auf der Website
    }

    public class AppPostViewModel
    {
        public List<MessageListItem> Messages { get; set; } = new();
        public int UnreadCount { get; set; }
        public bool HasCharacter { get; set; } = true;
        public bool IsSentFolder { get; set; }
    }

    /// <summary>Gesprächsverlauf einer Nachricht</summary>
    public class AppMessageThreadViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string OtherName { get; set; } = string.Empty;
        public List<AppMessageItem> Messages { get; set; } = new();

        // Antwort geht an …
        public int? ReplyCharacterId { get; set; }
        public string? ReplyUserId { get; set; }
        public int? ReplyType { get; set; }               // MessageSenderType bei Gott/Moderator
        public int ParentMessageId { get; set; }
        public string ReplySubject { get; set; } = string.Empty;
        public bool CanReply { get; set; }
    }

    public class AppMessageItem
    {
        public string SenderName { get; set; } = string.Empty;
        public bool IsMine { get; set; }
        public DateTime SentAt { get; set; }
        public string Content { get; set; } = string.Empty;   // reiner Text wie auf der Website
    }

    /// <summary>Neue Nachricht</summary>
    public class AppComposeViewModel
    {
        public List<AppReceiverOption> Receivers { get; set; } = new();
        public string? SenderName { get; set; }
    }

    public class AppReceiverOption
    {
        public string Value { get; set; } = string.Empty;     // "char:12" oder "user:<id>:1"
        public string Name { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;     // "Charaktere" / "Spielleitung"
    }

    public class AppOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AppCharacterViewModel
    {
        public int Id { get; set; }
        public List<AppOption> MyCharacters { get; set; } = new();
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public string Rufname { get; set; } = string.Empty;
        public string? Geburtsdatum { get; set; }
        public string? ImagePath { get; set; }
        public bool IsCompanion { get; set; }

        public string? Beruf { get; set; }
        public int? BodyHeight { get; set; }
        public string? Quote { get; set; }
        public string? Urheber { get; set; }
        public string? Description { get; set; }
        public int? StandId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }

        public int? GuildId { get; set; }
        public int? RegimentId { get; set; }
        public int? InfanterierangId { get; set; }
        public int? ReligionId { get; set; }

        public List<string> Magic { get; set; } = new();

        public List<AppOption> Staende { get; set; } = new();
        public List<AppOption> Blutgruppen { get; set; } = new();
        public List<AppOption> Haeuser { get; set; } = new();
        public List<AppOption> Herkunftslaender { get; set; } = new();
        public List<AppOption> Guilds { get; set; } = new();
        public List<AppOption> Regiments { get; set; } = new();
        public List<AppOption> Infanterieraenge { get; set; } = new();
        public List<AppOption> Religions { get; set; } = new();
    }

    /// <summary>Formular "Charakter speichern" aus der App</summary>
    public class AppCharacterSaveModel
    {
        public int Id { get; set; }
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public string Rufname { get; set; } = string.Empty;
        public string? Geburtsdatum { get; set; }
        public string? Beruf { get; set; }
        public int? BodyHeight { get; set; }
        public string? Quote { get; set; }
        public string? Urheber { get; set; }
        public string? Description { get; set; }
        public int? StandId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }
        public int? GuildId { get; set; }
        public int? RegimentId { get; set; }
        public int? InfanterierangId { get; set; }
        public int? ReligionId { get; set; }
    }

    /// <summary>Wiki-Eintrag zum Nachlesen</summary>
    public class AppEntryViewModel
    {
        public string TypeLabel { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? ImagePath { get; set; }
        public string? Quote { get; set; }
        public string? QuoteBy { get; set; }
        public string? DescriptionHtml { get; set; }
        public string? Notice { get; set; }
        public string FullPageUrl { get; set; } = string.Empty;
    }
}
