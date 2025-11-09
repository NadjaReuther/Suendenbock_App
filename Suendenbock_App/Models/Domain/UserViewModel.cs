namespace Suendenbock_App.Models.Domain
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "keine";
        public int CharacterCount { get; set; } = 0;

        // NEU: Triggerpunkte-Informationen
        public DateTime? Birthday { get; set; }
        public string? CustomTrigger { get; set; }
        public int TriggerPreferencesCount { get; set; } = 0;
    }
}