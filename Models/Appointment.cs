using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models
{
    public class Appointment
    {


        [Key]
        public int AppointmentId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public DateTime DateTime { get; set; }

        public string Status { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
