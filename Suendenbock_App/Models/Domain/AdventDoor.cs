namespace Suendenbock_App.Models.Domain
{
    public class AdventDoor
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }

        /// <summary>
        /// Typ des Türchens (Simple, Choice, DirectAudio)
        /// </summary>
        public DoorType DoorType { get; set; } = DoorType.Simple;

        // === Für Typ: Simple ===
        /// <summary>
        /// Pfad zur HTML-Datei (z.B. "/content/advent/day1.html")
        /// </summary>
        public string? HtmlContentPath { get; set; }

        // === Für Typ: Choice (Emma vs Kasimir) ===
        /// <summary>
        /// Audio-Datei die abgespielt wird wenn Emma gewählt wurde
        /// </summary>
        public string? EmmaAudioPath { get; set; }

        /// <summary>
        /// Audio-Datei die abgespielt wird wenn Kasimir gewählt wurde
        /// </summary>
        public string? KasimirAudioPath { get; set; }

        // === Für Typ: DirectAudio ===
        /// <summary>
        /// Pfad zur Audio-Datei die direkt abgespielt wird (z.B. "/audio/advent/day1.mp3")
        /// </summary>
        public string? AudioPath { get; set; }

        // Navigation
        public List<UserAdventChoice> UserChoices { get; set; }
    }
}
