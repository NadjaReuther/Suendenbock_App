using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class ForumCategory
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Icon { get; set; } = string.Empty ;
        [Required]
        public int SortOrder { get; set; }

        // Navigation Properties
        public List<ForumThread> Threads { get; set; } = new List<ForumThread>();
    }
}
