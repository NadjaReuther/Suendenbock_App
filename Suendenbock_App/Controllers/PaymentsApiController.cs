using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
    [Route("api/payments")]
    [ApiController]
    public class PaymentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/payments?year=2025&month=1
        [HttpGet]
        public async Task<IActionResult> GetPayments([FromQuery] int? year, [FromQuery] int? month)
        {
            var targetYear = year ?? DateTime.Now.Year;
            var targetMonth = month ?? DateTime.Now.Month;

            var payments = await _context.MonthlyPayments
                .Where(mp => mp.Year == targetYear && mp.Month == targetMonth)
                .OrderBy(mp => mp.PlayerName)
                .ToListAsync();

            return Ok(payments);
        }

        // POST: api/payments
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlayerName))
            {
                return BadRequest("Spielername erforderlich.");
            }

            // Check if payment already exists for this player in this month
            var exists = await _context.MonthlyPayments
                .AnyAsync(mp => mp.PlayerName == request.PlayerName
                    && mp.Year == request.Year
                    && mp.Month == request.Month);

            if (exists)
            {
                return BadRequest("Zahlung für diesen Spieler in diesem Monat existiert bereits.");
            }

            var payment = new MonthlyPayment
            {
                PlayerName = request.PlayerName,
                Year = request.Year,
                Month = request.Month,
                Status = request.Status ?? "unpaid",
                PaymentMethod = request.PaymentMethod,
                PaidAt = request.Status == "paid" ? DateTime.Now : null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.MonthlyPayments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // PUT: api/payments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentRequest request)
        {
            var payment = await _context.MonthlyPayments.FindAsync(id);

            if (payment == null)
            {
                return NotFound("Zahlung nicht gefunden.");
            }

            // Track old status to determine if we need to update PaidAt
            var oldStatus = payment.Status;

            payment.Status = request.Status;
            payment.PaymentMethod = request.PaymentMethod;
            payment.UpdatedAt = DateTime.Now;

            // Set PaidAt when status changes to paid
            if (request.Status == "paid" && oldStatus != "paid")
            {
                payment.PaidAt = DateTime.Now;
            }
            else if (request.Status == "unpaid")
            {
                payment.PaidAt = null;
            }

            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // DELETE: api/payments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.MonthlyPayments.FindAsync(id);

            if (payment == null)
            {
                return NotFound("Zahlung nicht gefunden.");
            }

            _context.MonthlyPayments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/payments/initialize-month
        [HttpPost("initialize-month")]
        public async Task<IActionResult> InitializeMonth([FromBody] InitializeMonthRequest request)
        {
            // Check if payments already exist for this month
            var exists = await _context.MonthlyPayments
                .AnyAsync(mp => mp.Year == request.Year && mp.Month == request.Month);

            if (exists)
            {
                return BadRequest("Zahlungen für diesen Monat existieren bereits.");
            }

            // Create payment entries for all players
            var payments = request.PlayerNames.Select(name => new MonthlyPayment
            {
                PlayerName = name,
                Year = request.Year,
                Month = request.Month,
                Status = "unpaid",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ToList();

            _context.MonthlyPayments.AddRange(payments);
            await _context.SaveChangesAsync();

            return Ok(payments);
        }
    }

    // Request Models
    public class CreatePaymentRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class UpdatePaymentRequest
    {
        public string Status { get; set; } = "unpaid";
        public string? PaymentMethod { get; set; }
    }

    public class InitializeMonthRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<string> PlayerNames { get; set; } = new();
    }
}
