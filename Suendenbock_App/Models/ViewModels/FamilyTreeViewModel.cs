using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class FamilyTreeViewModel
    {
        public Character RootCharacter { get; set; } = null!;
        public List<Character> Ancestors { get; set; } = new List<Character>();
        public List<Character> Descendants { get; set; } = new List<Character>();
        public List<Character> Siblings { get; set; } = new List<Character>();
    }
}
