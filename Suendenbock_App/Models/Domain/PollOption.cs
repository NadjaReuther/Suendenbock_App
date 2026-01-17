using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class PollOption
    {
        public int Id  { get; set; }
        [Required]
        public int PollId { get; set; }
        [Required(ErrorMessage = "Antworten sind erforderlich")]
        [StringLength(200)]
        public string Text { get; set; } =string.Empty;
        [Required]
        public int SortOrder { get; set; }

        //Navigation Properties
        public Poll Poll { get; set; } = null!;
        public List<PollVote> Votes { get; set; } = new List<PollVote>();

        // computed Property
        [NotMapped] 
        public int  VoteCount => Votes?.Count ?? 0;
    }
}
