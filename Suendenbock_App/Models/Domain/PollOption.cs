namespace Suendenbock_App.Models.Domain
{
    public class PollOption
    {
        public int Id  { get; set; }
        public int PollId { get; set; }
        public string Text { get; set; }

        //Navigation Properties
        public Poll Poll { get; set; }
        public List<PollVote> Votes { get; set; }
    }
}
