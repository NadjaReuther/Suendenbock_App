namespace Suendenbock_App.Models.ViewModels
{
    public class PlayerRiddleStats
    {
        public string UserId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = "Unbekannt";
        public int Solved { get; set; }
        public int TotalAttempts { get; set; }
        public int HintsUsed { get; set; }
    }
}
