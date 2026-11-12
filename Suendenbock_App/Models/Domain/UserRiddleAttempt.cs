namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Tracking für User-Versuche bei Rätsel-Türchen
    /// </summary>
    public class UserRiddleAttempt
    {
        public int Id { get; set; }

        /// <summary>
        /// User der das Rätsel versucht hat
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Welches Türchen
        /// </summary>
        public int RiddleDoorId { get; set; }
        public RiddleDoor RiddleDoor { get; set; } = null!;

        /// <summary>
        /// Anzahl der verwendeten Hinweise (0-3)
        /// </summary>
        public int HintsUsed { get; set; } = 0;

        /// <summary>
        /// Anzahl der Lösungsversuche (wie oft wurde "Prüfen" geklickt)
        /// </summary>
        public int AttemptCount { get; set; } = 0;

        /// <summary>
        /// Wurde das Rätsel erfolgreich gelöst?
        /// </summary>
        public bool IsSolved { get; set; } = false;

        /// <summary>
        /// Wann wurde das Türchen zuerst geöffnet?
        /// </summary>
        public DateTime FirstOpenedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Wann wurde das Rätsel gelöst? (null wenn noch nicht gelöst)
        /// </summary>
        public DateTime? SolvedAt { get; set; }

        /// <summary>
        /// Liste aller eingegebenen Lösungsversuche (für Statistik)
        /// Wird als JSON gespeichert
        /// </summary>
        public string? SubmittedAnswers { get; set; }
    }
}
