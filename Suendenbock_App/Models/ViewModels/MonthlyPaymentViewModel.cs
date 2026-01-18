namespace Suendenbock_App.Models.ViewModels
{
    public class MonthlyPaymentViewModel
    {
        public int Id { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Status { get; set; } = "unpaid"; // "paid" or "unpaid"
        public string? PaymentMethod { get; set; }
        public string? PaidAtDisplay { get; set; }
    }
}
