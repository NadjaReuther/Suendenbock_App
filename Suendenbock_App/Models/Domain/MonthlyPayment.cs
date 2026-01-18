using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class MonthlyPayment
    {
        public int Id { get; set; }

        [Required]
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; } // 1-12

        [Required]
        public string Status { get; set; } = "unpaid"; // "paid" or "unpaid"

        public string? PaymentMethod { get; set; } // "PayPal", "Überweisung", etc.

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
