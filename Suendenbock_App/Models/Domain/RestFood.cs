// Models/RestFood.cs
using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models
{
    /// <summary>
    /// Essen für die Rast - verschiedene Qualitätsstufen geben unterschiedliche Health-Boni
    /// </summary>
    public class RestFood
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Name ist erforderlich")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Wie viel Health wird wiederhergestellt?
        /// verdorben = -40, nichts = -20, schlecht = 0, billig = 10, gut = 30, edel = 50
        /// </summary>
        public int HealthBonus { get; set; } = 0;
    }
}