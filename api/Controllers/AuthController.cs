using Microsoft.AspNetCore.Mvc;
using ApartmentBooking.Services;
using ApartmentBooking.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService auth_service)
    {
        _authService = auth_service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Password);
        if (!result) return BadRequest("Couldn't register user.");
        return Ok("Registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password);
        if (token is null) return Unauthorized();
        return Ok(new { token });
    }
}
