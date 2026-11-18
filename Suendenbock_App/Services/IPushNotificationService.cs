namespace Suendenbock_App.Services
{
    /// <summary>
    /// Service-Interface für Push-Benachrichtigungen
    /// </summary>
    public interface IPushNotificationService
    {
        /// <summary>
        /// Sendet eine Push-Benachrichtigung an alle Benutzer die für einen bestimmten Typ abonniert sind
        /// </summary>
        /// <param name="notificationType">Art der Benachrichtigung (ForumThread, ForumReply, News, Poll)</param>
        /// <param name="title">Titel der Benachrichtigung</param>
        /// <param name="message">Nachrichtentext</param>
        /// <param name="url">URL zum Öffnen beim Klick</param>
        /// <param name="excludeUserId">Optional: UserId die keine Benachrichtigung erhalten soll (z.B. der Autor selbst)</param>
        Task SendNotificationAsync(string notificationType, string title, string message, string url, string? excludeUserId = null);

        /// <summary>
        /// Sendet eine Push-Benachrichtigung nur an die Geräte eines einzelnen Benutzers
        /// (z.B. Empfänger einer persönlichen Nachricht)
        /// </summary>
        /// <param name="userId">UserId des Empfängers</param>
        /// <param name="title">Titel der Benachrichtigung</param>
        /// <param name="message">Nachrichtentext</param>
        /// <param name="url">URL zum Öffnen beim Klick</param>
        /// <param name="notificationType">Optional: "Event" oder "Payment". Dann wird der passende Schalter
        /// in den Einstellungen des Benutzers beachtet. Ohne Typ (z.B. persönliche Nachricht) wird immer gesendet.</param>
        Task SendToUserAsync(string userId, string title, string message, string url, string? notificationType = null);

        /// <summary>
        /// Sendet eine Test-Benachrichtigung an einen spezifischen Benutzer
        /// </summary>
        Task SendTestNotificationAsync(string userId);
    }
}
