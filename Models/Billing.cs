using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hospital_Management.Models
{
    public class Billing
    {

        [Key]
        public int? BillId { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentStatus { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
