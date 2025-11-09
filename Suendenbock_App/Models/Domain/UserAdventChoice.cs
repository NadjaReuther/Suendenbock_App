namespace Suendenbock_App.Models.Domain
{
    public class UserAdventChoice
    {
        public int Id { get; set; }
        public string UserId { get; set; } // welcher User
        public int AdventDoorId { get; set; } // welches Türchen
        public int ChoiceIndex { get; set; } // wer wurde gewählt hinter dem Türchen
        public DateTime ChosenAt { get; set; } // wann gewählt

        //Navigation
        public AdventDoor AdventDoor { get; set; }
    }
}
