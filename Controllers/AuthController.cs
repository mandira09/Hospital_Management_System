using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public IActionResult Register(UserRegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password,
            Role = dto.Role
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User Registered");
    }

    [HttpPost("login")]
    public IActionResult Login(UserLoginDto dto)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Username == dto.Username && x.Password == dto.Password);

        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(user);

        return Ok(token);
    }
}