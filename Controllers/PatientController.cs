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
    [Authorize]
    [ApiController]
    [Route("api/patients")]

    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LoggerService _logger;

        public PatientController(AppDbContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult AddPatient(PatientDto dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            var patient = new Patient
            {
                Name = dto.Name,
                Age = dto.Age,
                Gender = dto.Gender,
                Contact = dto.Contact,
                Email = dto.Email,

                CreatedBy = username,
                CreatedAt = DateTime.Now
            };


            _context.Patients.Add(patient);
            _context.SaveChanges();

            stopwatch.Stop();

            _logger.Log(
        username,
        "Added Patient",
        stopwatch.ElapsedMilliseconds
    );

            return Ok(patient);
        }

        [HttpGet("{id}")]
        public IActionResult GetPatient(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var patient = _context.Patients.Find(id);

            stopwatch.Stop();

            _logger.Log(
        username,
        "Viewed a patient",
        stopwatch.ElapsedMilliseconds
    );
            return Ok(patient);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]   
        public IActionResult GetAllPatients()
        {
            var stopwatch = Stopwatch.StartNew();

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";


            var patients = _context.Patients.ToList();

            stopwatch.Stop();

            _logger.Log(
        username,
        "Viewed All Patients",
        stopwatch.ElapsedMilliseconds
    );
            return Ok(patients);
        }
    }
}
