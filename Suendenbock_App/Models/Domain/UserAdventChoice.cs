namespace Suendenbock_App.Models.Domain
{
    public class UserAdventChoice
    {
        public int Id { get; set; }
        public string UserId { get; set; } // welcher User
        public int AdventDoorId { get; set; } // welches Türchen

        /// <summary>
        /// Welche Auswahl wurde getroffen (0 = Emma, 1 = Kasimir)
        /// NULL = Türchen wurde nur geöffnet (Simple/DirectAudio)
        /// </summary>
        public int? ChoiceIndex { get; set; }

        public DateTime ChosenAt { get; set; } // wann geöffnet/gewählt

        //Navigation
        public AdventDoor AdventDoor { get; set; }
    }
}
