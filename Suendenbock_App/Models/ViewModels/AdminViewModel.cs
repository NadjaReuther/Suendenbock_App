using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<MagicClass> MagicClasses { get; set; } = new List<MagicClass>();
        public List<MagicClassSpecialization> Specializations { get; set; } = new List<MagicClassSpecialization>();
        public List<Guild> Guilds { get; set; } = new List<Guild>();
        public List<Religion> Religions { get; set; } = new List<Religion>();
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Infanterie> Infanteries { get; set; } = new List<Infanterie>();
        public List<Regiment> Regiments { get; set; } = new List<Regiment>();
    }
}
