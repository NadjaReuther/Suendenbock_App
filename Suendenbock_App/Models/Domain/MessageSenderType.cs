namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Typ des Nachrichten-Absenders
    /// </summary>
    public enum MessageSenderType
    {
        /// <summary>
        /// Nachricht von einem Charakter (normaler Spieler)
        /// </summary>
        Character = 0,

        /// <summary>
        /// Nachricht von Gott (Admin)
        /// </summary>
        Gott = 1,

        /// <summary>
        /// Nachricht von einem Moderator
        /// </summary>
        Moderator = 2
    }
}
