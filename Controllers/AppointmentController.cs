using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital_Management.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace Hospital_Management.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LoggerService _logger;

        public AppointmentController(AppDbContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult BookAppointment(AppointmentDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            if (dto.PatientId <= 0 || dto.DoctorId <= 0)
                return BadRequest("Invalid PatientId or DoctorId");

            var inputTime = dto.DateTime;
            if (inputTime < DateTime.Now)
            {
                return BadRequest("Cannot book appointment for past date/time");
            }

            if (inputTime.Hour < 10 || inputTime.Hour >= 17)
                return BadRequest("Doctor available only between 10AM - 5PM");

            // 🔥 Doctor unavailable for next 30 mins

            var appointmentEndTime = inputTime.AddMinutes(30);

            var exists = _context.Appointments.Any(a =>
                a.DoctorId == dto.DoctorId &&

                // overlap validation
                inputTime < a.DateTime.AddMinutes(30) &&
                appointmentEndTime > a.DateTime
            );

            if (exists)
            {
                return BadRequest("Doctor not available for next 30 minutes");
            }

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,   // ✅ DIRECT INPUT
                DoctorId = dto.DoctorId,
                DateTime = inputTime,
                Status = "Scheduled",

                CreatedBy = username,
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            stopwatch.Stop();

            _logger.Log(
                username,
                "Booked Appointment",
                stopwatch.ElapsedMilliseconds
            );

            return Ok("Appointment booked successfully");
        }

        [HttpGet("{patientId}")]
        public IActionResult Get(int patientId)
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var appointments = _context.Appointments
            .Where(x => x.PatientId == patientId)
            .ToList();

            stopwatch.Stop();

            // 🔥 Write into logs.txt
            _logger.Log(
                username,
                "Viewed Patient Appointments",
                stopwatch.ElapsedMilliseconds
            );

            return Ok(appointments);

        }
        [HttpPut("reschedule")]
        public IActionResult Reschedule(int appointmentId, DateTime newDateTime)
        {
            var stopwatch = Stopwatch.StartNew();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            var appointment = _context.Appointments.Find(appointmentId);

            if (appointment == null)
                return NotFound("Appointment not found");

            var exists = _context.Appointments.Any(x =>
                x.DoctorId == appointment.DoctorId &&
                x.DateTime == newDateTime);

            if (exists)
                return BadRequest("Doctor already booked for this time");

            appointment.DateTime = newDateTime;
            appointment.Status = "Rescheduled";
            appointment.UpdatedBy = username;
            appointment.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            stopwatch.Stop();
            _logger.Log(
    username,
    "Rescheduled Appointment",
    stopwatch.ElapsedMilliseconds
);

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient,Admin")]
        public IActionResult Delete(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound("Appointment not found");

            if (User.IsInRole("Admin"))
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
                stopwatch.Stop();
                _logger.Log(
    username,
    "Deleted Appointment (Admin)",
    stopwatch.ElapsedMilliseconds
);

                return Ok("Deleted by Admin");
            }

            var patientIdClaim = User.FindFirst("PatientId")?.Value;

            if (patientIdClaim == null)
                return Unauthorized("PatientId not found in token");

            int patientId = int.Parse(patientIdClaim);

            // 🔥 4. SECURITY CHECK
            if (appointment.PatientId != patientId)
            {
                return Forbid("You can only delete your own appointments");
            }

            // 🔥 5. Delete
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            stopwatch.Stop();

            // 🔥 Log into txt file
            _logger.Log(
                username,
                "Deleted Appointment",
                stopwatch.ElapsedMilliseconds
            );

            return Ok("Deleted successfully");
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]   
        public IActionResult GetByDoctor(int doctorId)
        {
            var stopwatch = Stopwatch.StartNew();

            // 🔥 Get logged-in username
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var appointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToList();

            stopwatch.Stop();

            // 🔥 Log into txt file
            _logger.Log(
                username,
                "Viewed Doctor Appointments",
                stopwatch.ElapsedMilliseconds
            );

            return Ok(appointments);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]    
        public IActionResult GetAll()

        {
            var stopwatch = Stopwatch.StartNew();

            // 🔥 Get logged-in username
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var appointments = _context.Appointments.ToList();

            stopwatch.Stop();

            // 🔥 Log into txt file
            _logger.Log(
                username,
                "Viewed All Appointments",
                stopwatch.ElapsedMilliseconds
            );

            return Ok(appointments);
        }

    }
}
