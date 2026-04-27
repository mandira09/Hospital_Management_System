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
    [Route("api/patients")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddPatient(PatientDto dto)
        {
            var patient = new Patient
            {
                Name = dto.Name,
                Age = dto.Age,
                Gender = dto.Gender,
                Contact = dto.Contact
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();

            return Ok(patient);
        }

        [HttpGet("{id}")]
        public IActionResult GetPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            return Ok(patient);
        }
    }
}
