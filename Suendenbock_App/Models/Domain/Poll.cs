namespace Suendenbock_App.Models.Domain
{
    public class Poll
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "active";
        public string Category { get; set; }
        public bool AllowMultipleChoices { get; set; } = false;
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ClosedAt { get; set; }

        //Navigation Properties
        public ApplicationUser CreatedBy { get; set; }
        List<PollOption> Options { get; set; }
        List<PollVote> Votes { get; set; }
    }
}
