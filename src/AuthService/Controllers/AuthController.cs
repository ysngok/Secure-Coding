using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Shared.Models;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public AuthController(AuthenticationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        try
        {
            var result = await _authService.RegisterAsync(user);
            return Ok(ApiResponse<User>.Ok(result, "User registered successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Registration failed", ex));
        }
    }

    /// <summary>
    /// Authenticate a user with username and password.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var loginDoc = BsonDocument.Parse(body);

            var (user, error) = await _authService.LoginAsync(loginDoc);

            if (user == null)
            {
                return Unauthorized(ApiResponse.Fail(error!));
            }

            return Ok(ApiResponse<object>.Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.IsAdmin,
                Token = "fake-jwt-token-not-implemented"
            }, "Login successful"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Login failed", ex));
        }
    }

    /// <summary>
    /// Search users by username.
    /// </summary>
    [HttpGet("users/search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest(ApiResponse.Fail("Search query is required"));

            var users = await _authService.SearchUsersAsync(q);
            return Ok(ApiResponse<List<User>>.Ok(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Search failed", ex));
        }
    }
}
