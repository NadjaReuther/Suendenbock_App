using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<MagicClass> MagicClasses { get; set; } = new List<MagicClass>();
        public List<Guild> Guilds { get; set; } = new List<Guild>();
    }
}
