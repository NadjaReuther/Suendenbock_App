using Microsoft.Build.ObjectModelRemoting;

namespace Suendenbock_App.Models.Domain
{
    public class PollVote
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int PollOptionId { get; set; }
        public int CharacterId { get; set; }
        public DateTime VoteAt { get; set; }

        //Navigation Properties
        public Poll Poll {  get; set; }
        public PollOption PollOption { get; set; }
        public Character Character { get; set; }
    }
}
