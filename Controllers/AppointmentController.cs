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
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Book(AppointmentDto dto)
        {
            var exists = _context.Appointments.Any(x =>
                x.DoctorId == dto.DoctorId &&
                x.DateTime == dto.DateTime);

            if (exists)
                return BadRequest("Doctor already booked");

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                DateTime = dto.DateTime,
                Status = "Scheduled" // 🔥 auto set
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return Ok(appointment);
        }

        [HttpGet("{patientId}")]
        public IActionResult Get(int patientId)
        {
            return Ok(_context.Appointments
                .Where(x => x.PatientId == patientId)
                .ToList());
        }
        [HttpPut("reschedule")]
        public IActionResult Reschedule(int appointmentId, DateTime newDateTime)
        {
            var appointment = _context.Appointments.Find(appointmentId);

            if (appointment == null)
                return NotFound("Appointment not found");

            // check double booking again
            var exists = _context.Appointments.Any(x =>
                x.DoctorId == appointment.DoctorId &&
                x.DateTime == newDateTime);

            if (exists)
                return BadRequest("Doctor already booked for this time");

            appointment.DateTime = newDateTime;
            appointment.Status = "Rescheduled";

            _context.SaveChanges();

            return Ok(appointment);
        }
        [HttpDelete]
        public IActionResult Delete(int appointmentId)
        {
            var appointment = _context.Appointments.Find(appointmentId);

            if (appointment == null)
                return NotFound("Appointment not found");

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return Ok("Appointment cancelled");
        }

    }
}
