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
        public const string BoxenEinraeumen = "Boxen wieder einräumen";
        public const string MuellSammeln = "Müll sammeln";
        public const string Schmeckiedienst = "Schmeckiedienst";
        public const string ZurueckInsLager = "Alles zurück ins Lager";
        public const string Mittagshilfe = "Mittagshilfe";
        public const string Fruehschicht = "Frühschicht";
        public const string Geschirrdienst = "Geschirrdienst";
        public const string Muellentsorgung = "Müllentsorgung";
        public const string Tafel = "Tafel";

        public static readonly string[] All = { BoxenEinraeumen, MuellSammeln, Schmeckiedienst, 
            ZurueckInsLager, Mittagshilfe, Fruehschicht, Geschirrdienst, Muellentsorgung, Tafel };

        public static readonly string[] Special = { Fruehschicht, Schmeckiedienst };
    }
}
