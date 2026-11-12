namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Rätsel-Türchen für den Adventskalender
    /// </summary>
    public class RiddleDoor
    {
        public int Id { get; set; }

        /// <summary>
        /// Tag-Nummer (1-24)
        /// </summary>
        public int DayNumber { get; set; }

        /// <summary>
        /// Pfad zum Türchen-Bild (z.B. "/assets/raetsel/tuer1.png")
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Das korrekte Lösungswort (case-insensitive Vergleich)
        /// </summary>
        public string Solution { get; set; } = string.Empty;

        /// <summary>
        /// Erster Hinweis
        /// </summary>
        public string Hint1 { get; set; } = string.Empty;

        /// <summary>
        /// Zweiter Hinweis
        /// </summary>
        public string Hint2 { get; set; } = string.Empty;

        /// <summary>
        /// Dritter Hinweis
        /// </summary>
        public string Hint3 { get; set; } = string.Empty;

        /// <summary>
        /// Optional: Zusätzliche Beschreibung/Kontext zum Rätsel
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Ist dieses Türchen aktiv/sichtbar?
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public List<UserRiddleAttempt> UserAttempts { get; set; } = new List<UserRiddleAttempt>();
    }
}
