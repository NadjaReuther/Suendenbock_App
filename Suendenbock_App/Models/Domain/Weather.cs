// Models/Domain/Weather.cs
using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Wetter-Optionen für einen bestimmten Monat
    /// </summary>
    public class WeatherOption
    {
        public int Id { get; set; }

        /// <summary>
        /// Monat (z.B. "Januar", "Februar", etc.)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Month { get; set; } = string.Empty;

        /// <summary>
        /// Wetterbezeichnung (z.B. "Kalter, klarer Himmel")
        /// </summary>
        [Required]
        [StringLength(200)]
        public string WeatherName { get; set; } = string.Empty;

        /// <summary>
        /// 5-Tage Wettervorhersage für diese Option
        /// </summary>
        public List<WeatherForecastDay> ForecastDays { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Ein einzelner Tag in der 5-Tage Wettervorhersage
    /// </summary>
    public class WeatherForecastDay
    {
        public int Id { get; set; }

        /// <summary>
        /// Zu welcher Wetter-Option gehört dieser Tag?
        /// </summary>
        public int WeatherOptionId { get; set; }
        public WeatherOption WeatherOption { get; set; } = null!;

        /// <summary>
        /// Tag (z.B. "Mo", "Di", "Mi", etc.)
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Day { get; set; } = string.Empty;

        /// <summary>
        /// Wetter-Icon als Unicode/Emoji (z.B. "☀️", "🌧️", "❄️")
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Temperatur (z.B. "2°/-5°")
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Temperature { get; set; } = string.Empty;

        /// <summary>
        /// Reihenfolge (0-4 für 5 Tage)
        /// </summary>
        public int DayOrder { get; set; }
    }
}
