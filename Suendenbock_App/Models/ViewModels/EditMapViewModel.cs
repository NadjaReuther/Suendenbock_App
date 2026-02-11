using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class EditMapViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ActId { get; set; }
        public bool IsWorldMap { get; set; }
        public int? ParentMapId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Act> Acts { get; set; } = new List<Act>();
        public List<WorldMapOption> WorldMaps { get; set; } = new List<WorldMapOption>();

        public int ChildMapsCount { get; set; }
        public List<Map> ChildMaps { get; set; } = new List<Map>();
    }
}
