namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Definiert die verschiedenen Türchen-Typen im Adventskalender
    /// </summary>
    public enum DoorType
    {
        /// <summary>
        /// Einfaches Türchen mit HTML-Inhalt (Text und Bilder)
        /// </summary>
        Simple = 0,

        /// <summary>
        /// Auswahl-Türchen (z.B. Emma vs Kasimir) mit Bildern und optionalem Audio nach der Wahl
        /// </summary>
        Choice = 1,

        /// <summary>
        /// Türchen mit direktem Audio-Abspielen (ohne Auswahl)
        /// </summary>
        DirectAudio = 2
    }
}
