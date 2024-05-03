using System.Text.Json;
using DLS_Backend.Models;
using DLS_Backend.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.Controller;

/// <summary>
/// AuthApiController class for handling authentication requests
/// </summary>
[ApiController]
[Route("v1")]
public class AuthApiController : ControllerBase
{
    private readonly DlsUsersContext _context;
    private readonly JwtService _jwtService;
    private readonly Hashing hash;
    
    /// <summary>
    /// Constructor for the AuthApiController
    /// </summary>
    /// <param name="context"></param>
    /// <param name="jwtService"></param>
    public AuthApiController(DlsUsersContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
        hash = new Hashing();
    }
    
    /// <summary>
    /// Register method for the AuthApiController
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var hashedPassword = hash.Hash(request.Password); // Hash the password before storing

        var latestUserInfo = await _context.LatestUserInfosView
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (latestUserInfo != null)
        {
            return BadRequest("User already exists");
        }
    
        var user = new User
        {   
            guid = request.guid,
            created_at = DateTime.UtcNow,
        };

        var userInfo = new UserInfo
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = hashedPassword, // Store the hashed password
            created_at = DateTime.UtcNow, // Set the created time
        
        };
        await _context.SaveChangesAsync(); // Save changes in the DB context

        userInfo.Password = null; // Remove the password from the user object

        var mergedObject = new
        {
            user = new
            {
                guid = user.guid,
                created_at = user.created_at
            },
            userInfo = new
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Email = userInfo.Email,
                created_at = userInfo.created_at
            }
        };

        string mergedJson = JsonSerializer.Serialize(mergedObject);
    
        Console.WriteLine(mergedJson);
    

        var rabbitMQService = new RabbitMQService();
        rabbitMQService.SendMessage(mergedJson);
        rabbitMQService.Close();

        return CreatedAtRoute("GetUser", new { email = userInfo.Email }, user);
    }
    
    /// <summary>
    ///  Login method for the AuthApiController
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        
        var latestUserInfo = await _context.LatestUserInfosView
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (latestUserInfo == null)
        {
            return BadRequest("No userinfo was found");
        }
        
        var user = await _context.Users.FindAsync(latestUserInfo!.UserId);
        if (user == null)
        {
            return BadRequest("No user was found");
        }

        if (hash.Verify(request.Password, latestUserInfo!.Password))
        {
            var token = _jwtService.GenerateToken(user.guid);
            return Ok(new { token, user = new {user.guid, latestUserInfo.Email, latestUserInfo.FirstName, latestUserInfo.LastName}});
        }
        return Unauthorized();
    }
    
    /// <summary>
    /// GenerateToken method for the AuthApiController
    /// </summary>
    /// <returns></returns>
    [HttpGet("generate-token")]
    public IActionResult GenerateToken()
    {
        // Generating a token with a fictive GUID
        string token = _jwtService.GenerateToken(Guid.Parse("f79330ab-c0bd-4bf8-97f2-37718917f2c9"));

        // Console log the token
        Console.WriteLine($"Generated JWT token: {token}");

        // Returns the generated token as a HTTP-respons
        return Ok(new { token });
    }
    
}