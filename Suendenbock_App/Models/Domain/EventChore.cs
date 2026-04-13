using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class EventChore
    {
        public int Id { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        [StringLength(100)]
        public string ChoreName { get; set; } = string.Empty;
        [StringLength(100)]
        public string? AssignedToName { get; set; }
        public bool IsSpecial { get; set; } = false;

        //Navigation Properties
        public MonthlyEvent Event { get; set; } = null!;
    }

    public static class ChoreNames
    {
        public const string Spielmaterial = "Spielmaterial";
        public const string MuellUndLeergut = "Müll & Leergut";
        public const string Schmeckiedienst = "Schmeckiedienst";
        public const string TischUndRaum = "Tisch & Raum";
        public const string Mittagshilfe = "Mittagshilfe";
        public const string Fruehschicht = "Frühdienst";
        public const string Geschirr = "Geschirr";

        public static readonly string[] All = { Spielmaterial, MuellUndLeergut, Schmeckiedienst,
            TischUndRaum, Mittagshilfe, Fruehschicht, Geschirr };

        public static readonly string[] Special = { Fruehschicht, Schmeckiedienst };

        public static readonly Dictionary<string, string[]> Descriptions = new Dictionary<string, string[]>
        {
            { Spielmaterial, new[] {
                "Grimoires, Würfelsets, Notizzettel, Mappen und Stifte einsammeln",
                "Alles ordentlich in die Kisten",
                "Ausweisringe wieder in den Flur hängen",
                "SL-Tisch & Karten aufräumen"
            }},
            { MuellUndLeergut, new[] {
                "Herumliegenden Müll einsammeln",
                "Alle Mülleimer leeren und Beutel wechseln",
                "Leergut einsammeln und sortieren",
                "Müllbeutel runter bringen"
            }},
            { Schmeckiedienst, new[] {
                "Was richtig leckeres für Gott, gerne auch was zu Trinken"
            }},
            { TischUndRaum, new[] {
                "Tisch abwischen",
                "Decken zusammenfalten und wegräumen",
                "Fenster öffnen zum Lüften",
                "Tisch & Sofa wieder an seinen Platz schieben (SL hilft)"
            }},
            { Mittagshilfe, new[] {
                "Bleib während der Mittagspause zum Helfen",
                "Unterstütze beim Kochen und Vorbereiten",
                "Hilf beim Anrichten/Servieren",
                "Küche währenddessen ordentlich halten"
            }},
            { Fruehschicht, new[] {
                "2 Stunden vor Beginn da sein",
                "Frischen Kaffee & heißes Wasser kochen",
                "Gemüse & Obst schneiden",
                "Jedem ein Getränk hinstellen",
                "Küche für den Mittagsdienst sauber hinterlassen"
            }},
            { Geschirr, new[] {
                "Alle Teller, Tassen & Bestecke in die Küche bringen",
                "Töpfe und Pfannen vom Mittag nicht vergessen",
                "Wasserbehälter einsammeln",
                "Alles in die Spülmaschine auf dem 90° Programm"
            }}
        };
    }
}
