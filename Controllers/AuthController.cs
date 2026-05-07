using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Hospital_Management.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwt;
    private readonly EmailService _email;
    private readonly LoggerService _logger;

    public AuthController(AppDbContext context, JwtService jwt, EmailService email, LoggerService logger)
    {
        _context = context;
        _jwt = jwt;
        _email = email;
        _logger = logger;
    }

    // 🔥 REGISTER
    [HttpPost("register")]
    public IActionResult Register(UserRegisterDto dto)
    {
        
        var existingUser = _context.Users
            .FirstOrDefault(x => x.Username.ToLower() == dto.Username.ToLower());

        if (existingUser != null)
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password, // ⚠️ later hash it
            Role = dto.Role,
            Email = dto.Email
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        string body = $@"
        <h2>Registration Successful</h2>
        <p><b>Username:</b> {user.Username}</p>
        <p><b>Password:</b> {user.Password}</p>
        <p><b>User ID:</b> {user.Id}</p>
    ";

        _email.SendEmail(user.Email, "Registration Successful", body);

        return Ok("User Registered Successfully");
    }

    // 🔥 LOGIN
    [HttpPost("login")]
    public IActionResult Login(UserLoginDto dto)
    {
        var user = _context.Users
            .FirstOrDefault(x =>
                x.Username.ToLower() == dto.Username.ToLower() &&
                x.Password == dto.Password);

        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(user);

        return Ok(new
        {
            token = token,
            role = user.Role,
            userId = user.Id
        });
    }
}