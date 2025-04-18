namespace Suendenbock_App.Models
{
    public class AdminViewModel
    {
        public List<MagicClassModel> MagicClasses { get; set; }
        public List<GuildModel> Guilds { get; set; }
        public List<ReligionModel> Religions { get; set; }
        public List<CharacterModel> Characters { get; set; }
    }
}
