using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hospital_Management.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace Hospital_Management.Controllers
{
    
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LoggerService _logger;

        public DoctorController(AppDbContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddDoctor(DoctorDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            var doctor = new Doctor
            {
                Name = dto.Name,
                Specialization = dto.Specialization,
                Availability = dto.Availability,
                Email = dto.Email,
                CreatedBy = username,
                CreatedAt = DateTime.Now


            };

            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            stopwatch.Stop();

            _logger.Log(
       username,
       "Added Doctor",
       stopwatch.ElapsedMilliseconds
   );
            return Ok(doctor);
        }
        
        [HttpGet]
        public IActionResult GetDoctors()

        {
            var stopwatch = Stopwatch.StartNew();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            var doctors = _context.Doctors.ToList();
            stopwatch.Stop();

            _logger.Log(
        username,
        "Viewed Doctors",
        stopwatch.ElapsedMilliseconds
    );

            return Ok(doctors);

        }
    }
}
