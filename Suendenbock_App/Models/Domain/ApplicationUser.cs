using Microsoft.AspNetCore.Identity;

namespace Suendenbock_App.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }

        // Trigger-System
        public DateTime? Birthday { get; set; }
        public string? CustomTrigger { get; set; }
        public string? Farbcode { get; set; }

        public ICollection<UserTriggerPreference> TriggerPreferences { get; set; } = new List<UserTriggerPreference>();
    }
}