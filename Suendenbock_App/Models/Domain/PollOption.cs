using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class PollOption
    {
        public int Id  { get; set; }
        public int PollId { get; set; }
        [Required(ErrorMessage = "Antworten sind erforderlich")]
        [StringLength(200)]
        public string Text { get; set; } =string.Empty;

        //Navigation Properties
        public Poll? Poll { get; set; }
        public List<PollVote>? Votes { get; set; }
    }
}
