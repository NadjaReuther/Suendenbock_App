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
        /// Sendet eine Test-Benachrichtigung an einen spezifischen Benutzer
        /// </summary>
        Task SendTestNotificationAsync(string userId);
    }
}
