using Microsoft.AspNetCore.Mvc;
using RPSLS.Application.DTOs;
using RPSLS.Application.Interfaces;
using RPSLS.Domain.Entities;
using RPSLS.Application.Utils;

namespace RPSLS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        var users = _userRepository.GetAll();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<User> GetById(Guid id)
    {
        var user = _userRepository.GetById(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.HashedPassword))
            return BadRequest("Username and password are required.");

        user.Id = Guid.NewGuid();
        _userRepository.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPost("login")]
    public ActionResult<User> Login([FromBody] LoginRequestDTO login)
    {
        if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            return BadRequest("Username and password are required.");

        var user = _userRepository.GetByUsername(login.Username);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        if (!PasswordUtils.VerifyPassword(login.Password, user.HashedPassword))
            return Unauthorized("Invalid username or password.");

        return Ok(user);
    }

    [HttpPost("register")]
    public ActionResult<User> Register([FromBody] LoginRequestDTO register)
    {
        if (string.IsNullOrWhiteSpace(register.Username) || string.IsNullOrWhiteSpace(register.Password))
            return BadRequest("Username and password are required.");

        var existingUser = _userRepository.GetByUsername(register.Username);
        if (existingUser != null)
            return Conflict("Username already exists.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = register.Username,
            HashedPassword = hashedPassword
        };

        _userRepository.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}