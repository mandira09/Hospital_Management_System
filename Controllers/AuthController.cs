using Hospital_Management.Data;
using Hospital_Management.DTOs;
using Hospital_Management.Models;
using Hospital_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
    [Authorize(Roles = "Admin")]
    public IActionResult Register(UserRegisterDto dto)
    {
        var stopwatch = Stopwatch.StartNew();
        var existingUser = _context.Users
            .FirstOrDefault(x => x.Username.ToLower() == dto.Username.ToLower());

        if (existingUser != null)
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password, // ⚠️ later hash it
            Role = dto.Role,
            Email = dto.Email,
            UId = dto.UId,
            DId = dto.DId,



            CreatedBy = "Admin",
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        string userIdText = "";

        // 🔥 Patient
        if (user.Role.ToLower() == "patient")
        {
            userIdText = user.UId?.ToString() ?? "0";
        }

        // 🔥 Doctor
        else if (user.Role.ToLower() == "doctor")
        {
            userIdText = user.DId?.ToString() ?? "0";
        }
        string body = $@"
        <h2>Registration Successful</h2>
        <p><b>Username:</b> {user.Username}</p>
        <p><b>Password:</b> {user.Password}</p>
        <p><b>User ID:</b> {userIdText}</p>
        <p><b>Role:</b> {user.Role}</p>
    ";

        _email.SendEmail(user.Email, "Registration Successful", body);

        stopwatch.Stop();

        _logger.Log(
        User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin",
        "Registered User",
        stopwatch.ElapsedMilliseconds
    );

        return Ok("User Registered Successfully");
    }

    // 🔥 LOGIN
    [HttpPost("login")]
    public IActionResult Login(UserLoginDto dto)
    {
        var stopwatch = Stopwatch.StartNew();

        var user = _context.Users
            .FirstOrDefault(x =>
                x.Username.ToLower() == dto.Username.ToLower() &&
                x.Password == dto.Password);

        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(user);
        stopwatch.Stop();

        _logger.Log(
        user.Username,
        "User Login",
        stopwatch.ElapsedMilliseconds
    );

        object userId = null;

        if (user.Role.ToLower() == "patient")
        {
            userId = user.UId;
        }
        else if (user.Role.ToLower() == "doctor")
        {
            userId = user.DId;
        }

        return Ok(new
        {
            token = token,
            role = user.Role,
            userId = userId
        });
    }
}