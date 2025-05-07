using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<MagicClass> MagicClasses { get; set; } = new List<MagicClass>();
        public List<Specialization> Specializations { get; set; } = new List<Specialization>();
        public List<Guild> Guilds { get; set; } = new List<Guild>();
        public List<Religion> Religions { get; set; } = new List<Religion>();
        public List<Character> Characters { get; set; } = new List<Character>();
    }
}
