using Microsoft.Build.ObjectModelRemoting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class PollVote
    {
        public int Id { get; set; }
        [Required]
        public int PollId { get; set; }
        [Required]
        public int PollOptionId { get; set; }
        public int? CharacterId { get; set; }
        public string? UserId { get; set; }
        [Required]
        public DateTime VotedAt { get; set; }

        //Navigation Properties
        public Poll Poll {  get; set; } = null!;
        public PollOption PollOption { get; set; } = null!;
        public Character? Character { get; set; }
        public ApplicationUser? User { get; set; }

        // computed Property
        [NotMapped]
        public string VoterName => Character?.Vorname ?? User?.UserName ?? "Unbekannt";
    }
}
