using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class CreateMapViewModel
    {
        public int ActId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsWorldMap { get; set; }
        public string? RegionName { get; set; }
        public int? ParentMapId { get; set; }

        // For dropdowns
        public List<Act> Acts { get; set; } = new List<Act>();
        public List<WorldMapOption> WorldMaps { get; set; } = new List<WorldMapOption>();
    }

    public class WorldMapOption
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
