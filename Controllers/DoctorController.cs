using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddDoctor(DoctorDto dto)
        {
            var doctor = new Doctor
            {
                Name = dto.Name,
                Specialization = dto.Specialization,
                Availability = dto.Availability
            };

            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            return Ok(doctor);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetDoctors()
        {
            return Ok(_context.Doctors.ToList());
        }
    }
}
