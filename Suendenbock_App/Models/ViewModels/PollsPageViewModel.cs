namespace Suendenbock_App.Models.ViewModels
{
    public class PollsPageViewModel
    {
        public List<PollViewModel> Polls { get; set; } = new();
        public bool IsAdmin { get; set; }
    }

    public class PollViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool AllowMultipleChoices { get; set; }
        public int TotalVoters { get; set; }
        public List<PollOptionViewModel> Options { get; set; } = new();
        public List<int> UserVotedOptionIds { get; set; } = new();
        public List<string> VoterNames { get; set; } = new();
        public bool CanEdit { get; set; }
    }

    public class PollOptionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Votes { get; set; }
        public double Percentage { get; set; }
    }
}
