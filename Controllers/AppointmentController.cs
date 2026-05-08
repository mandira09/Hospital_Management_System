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

            // 🔥 PATIENT VALIDATION
            if (User.IsInRole("Patient"))
            {
                var uIdClaim = User.FindFirst("UId")?.Value;

                if (uIdClaim == null)
                    return Unauthorized("UId not found in token");

                int uId = int.Parse(uIdClaim);

                // 🔥 Patient can book only for own ID
                if (dto.PatientId != uId)
                {
                    return Unauthorized("You can book appointments only for yourself");
                }
            }

            if (dto.PatientId <= 0 || dto.DoctorId <= 0)
                return BadRequest("Invalid PatientId or DoctorId");

            var inputTime = dto.DateTime;

            // 🔥 Prevent past booking
            if (inputTime < DateTime.Now)
            {
                return BadRequest("Cannot book appointment for past date/time");
            }

            // 🔥 Doctor working hours
            if (inputTime.Hour < 10 || inputTime.Hour >= 17)
                return BadRequest("Doctor available only between 10AM - 5PM");

            // 🔥 Doctor unavailable for next 30 mins
            var appointmentEndTime = inputTime.AddMinutes(30);

            var exists = _context.Appointments.Any(a =>
                a.DoctorId == dto.DoctorId &&
                a.Status != "Cancelled" &&

                // 🔥 Overlap validation
                inputTime < a.DateTime.AddMinutes(30) &&
                appointmentEndTime > a.DateTime
            );

            if (exists)
            {
                return BadRequest("Doctor not available");
            }

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
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
        [Authorize(Roles = "Admin,Patient")]
        public IActionResult Get(int patientId)
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            // 🔥 ADMIN CAN VIEW ANY PATIENT APPOINTMENTS
            if (!User.IsInRole("Admin"))
            {
                var uIdClaim = User.FindFirst("UId")?.Value;

                if (uIdClaim == null)
                    return Unauthorized("UId not found in token");

                int uId = int.Parse(uIdClaim);

                // 🔥 Patient can view only own appointments
                if (patientId != uId)
                {
                    return Unauthorized("You can view only your own appointments");
                }
            }

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
        public IActionResult Reschedule(RescheduleDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var appointment = _context.Appointments.Find(dto.AppointmentId);

            if (appointment == null)
                return NotFound("Appointment not found");

            // 🔥 ADMIN CAN RESCHEDULE ANY APPOINTMENT
            if (!User.IsInRole("Admin"))
            {
                // 🔥 PATIENT VALIDATION
                if (User.IsInRole("Patient"))
                {
                    var uIdClaim = User.FindFirst("UId")?.Value;

                    if (uIdClaim == null)
                        return Unauthorized("UId not found in token");

                    int uId = int.Parse(uIdClaim);

                    // 🔥 Patient can reschedule only own appointment
                    if (appointment.PatientId != uId)
                    {
                        return Unauthorized("You can reschedule only your own appointments");
                    }
                }

                // 🔥 DOCTOR VALIDATION
                if (User.IsInRole("Doctor"))
                {
                    var dIdClaim = User.FindFirst("DId")?.Value;

                    if (dIdClaim == null)
                        return Unauthorized("DId not found in token");

                    int dId = int.Parse(dIdClaim);

                    // 🔥 Doctor can reschedule only own appointments
                    if (appointment.DoctorId != dId)
                    {
                        return Unauthorized("You can reschedule only your own appointments");
                    }
                }
            }

            // 🔥 Prevent past date/time
            if (dto.NewDateTime < DateTime.Now)
            {
                return BadRequest("Cannot reschedule to past date/time");
            }

            // 🔥 30-minute overlap validation
            var appointmentEndTime = dto.NewDateTime.AddMinutes(30);

            var exists = _context.Appointments.Any(x =>
                x.DoctorId == appointment.DoctorId &&
                x.AppointmentId != dto.AppointmentId &&
                x.Status != "Cancelled" &&
                // 🔥 Overlap check
                dto.NewDateTime < x.DateTime.AddMinutes(30) &&
                appointmentEndTime > x.DateTime
            );

            if (exists)
            {
                return BadRequest("Doctor already booked");
            }

            // 🔥 Update appointment
            appointment.DateTime = dto.NewDateTime;
            appointment.Status = "Rescheduled";

            // 🔥 Audit fields
            appointment.UpdatedBy = username;
            appointment.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            stopwatch.Stop();

            // 🔥 txt logging
            _logger.Log(
                username,
                "Rescheduled Appointment",
                stopwatch.ElapsedMilliseconds
            );

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public IActionResult Delete(int id)
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound("Appointment not found");

            // 🔥 ADMIN CAN CANCEL ANY APPOINTMENT
            if (User.IsInRole("Admin"))
            {
                appointment.Status = "Cancelled";

                appointment.UpdatedBy = username;
                appointment.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                stopwatch.Stop();

                _logger.Log(
                    username,
                    "Cancelled Appointment (Admin)",
                    stopwatch.ElapsedMilliseconds
                );

                return Ok("Appointment cancelled by Admin");
            }

            // 🔥 PATIENT VALIDATION
            if (User.IsInRole("Patient"))
            {
                var uIdClaim = User.FindFirst("UId")?.Value;

                if (uIdClaim == null)
                    return Unauthorized("UId not found in token");

                int uId = int.Parse(uIdClaim);

                // 🔥 Patient can cancel only own appointment
                if (appointment.PatientId != uId)
                {
                    return Unauthorized("You can cancel only your own appointments");
                }
            }

            // 🔥 DOCTOR VALIDATION
            if (User.IsInRole("Doctor"))
            {
                var dIdClaim = User.FindFirst("DId")?.Value;

                if (dIdClaim == null)
                    return Unauthorized("DId not found in token");

                int dId = int.Parse(dIdClaim);

                // 🔥 Doctor can cancel only own appointments
                if (appointment.DoctorId != dId)
                {
                    return Unauthorized("You can cancel only your own appointments");
                }
            }

            // 🔥 SOFT DELETE
            appointment.Status = "Cancelled";

            appointment.UpdatedBy = username;
            appointment.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            stopwatch.Stop();

            _logger.Log(
                username,
                "Cancelled Appointment",
                stopwatch.ElapsedMilliseconds
            );

            return Ok("Appointment cancelled successfully");
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public IActionResult GetByDoctor(int doctorId)
        {
            var stopwatch = Stopwatch.StartNew();

            // 🔥 Get logged-in username
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            // 🔥 ADMIN CAN VIEW ANY DOCTOR APPOINTMENTS
            if (!User.IsInRole("Admin"))
            {
                var dIdClaim = User.FindFirst("DId")?.Value;

                if (dIdClaim == null)
                    return Unauthorized("DId not found in token");

                int dId = int.Parse(dIdClaim);

                // 🔥 Doctor can view only own appointments
                if (doctorId != dId)
                {
                    return Unauthorized("You can view only your own appointments");
                }
            }

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
        [Authorize(Roles = "Admin")]    
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
