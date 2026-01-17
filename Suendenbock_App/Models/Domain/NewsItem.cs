using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class NewsItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [MaxLength(200)]
        public string Excerpt { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = "Spiel-Update";

        [Required]
        [MaxLength(50)]
        public string Icon { get; set; } = "campaign";

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<NewsComment> Comments { get; set; } = null!;
    }
}
