using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<MagicClass> MagicClasses { get; set; } = new List<MagicClass>();
        public List<Infanterie> Infanteries { get; set; } = new List<Infanterie>();
        public List<Guild> Guilds { get; set; } = new List<Guild>();
        public List<Character> recentCharacters { get; set; } = new List<Character>();
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Monster> Monsters { get; set; } = new List<Monster>();
        public List<Monstertyp> Monstertyps { get; set; } = new List<Monstertyp>();
        public Dictionary<string, int> MagicClassStats { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> GenderStats { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, List<string>> ZodiacStats { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, int> Obermagien { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, int>> MagicClassDetails { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        // Spielercharaktere und Begleiter
        public List<Character> PlayerCharacters { get; set; } = new List<Character>();
        public List<Character> CompanionCharacters { get; set; } = new List<Character>();
    }
}
