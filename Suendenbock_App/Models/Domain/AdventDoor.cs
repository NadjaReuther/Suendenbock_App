namespace Suendenbock_App.Models.Domain
{
    public class AdventDoor
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public string Question { get; set; }
        public string ImagePath { get; set; }

        // Die Auswahlausmöglichkeiten (JSON)
        public string ChoicesJson { get; set; }
        
        // Navigation
        public List<UserAdventChoice> UserChoices { get; set; }

    }
}
