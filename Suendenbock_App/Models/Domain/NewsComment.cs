using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class NewsComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public int NewsItemId { get; set; }

        [ForeignKey("NewsItemId")]
        public virtual NewsItem NewsItem { get; set; } = null!;
    }
}
