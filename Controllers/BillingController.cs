using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/billing")]
    public class BillingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BillingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateBill(BillingDto dto)
        {
            var bill = new Billing
            {
                AppointmentId = dto.AppointmentId,
                Amount = dto.Amount,
                PaymentStatus = "Pending" // 🔥 auto set
            };

            _context.Billings.Add(bill);
            _context.SaveChanges();

            return Ok(bill);
        }

        [HttpPost("payment")]
        [Authorize(Roles = "Admin")]
        public IActionResult Pay(int billId)
        {
            var bill = _context.Billings.Find(billId);

            if (bill == null)
                return NotFound("Bill not found");

            bill.PaymentStatus = "Paid";

            _context.SaveChanges();

            return Ok("Payment Successful");
        }

        [HttpGet("{patientId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetBills(int patientId)
        {
            var bills = _context.Billings
                .Where(b => _context.Appointments
                    .Any(a => a.AppointmentId == b.AppointmentId && a.PatientId == patientId))
                .ToList();

            return Ok(bills);
        }
    }
}
