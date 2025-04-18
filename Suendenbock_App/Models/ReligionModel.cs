namespace Suendenbock_App.Models
{
    public class ReligionModel
    {
        public int Id { get; set; }
        public string Type { get; set; }

        //Navigation Properties
        public virtual ICollection<CharacterModel> Characters { get; set; } // Religion has many Characters

    }
}
