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

        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Save changes in the DB context to generate the id for the user

        var userInfo = new UserInfo
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = hashedPassword, // Store the hashed password
            created_at = DateTime.UtcNow, // Set the created time
            UserId = user.id // Set the UserId to the id of the user
        };

        _context.UserInfo.Add(userInfo);
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
        string queueName = "UserQueue";
        rabbitMQService.SendMessage(queueName,mergedJson);
        rabbitMQService.Close();

        return StatusCode(201, mergedJson);
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
            
            var userTombstone = await _context.UserTombstones.FindAsync(user.id);
            if (userTombstone != null)
            {
                // If there is, remove it
                _context.UserTombstones.Remove(userTombstone);
                await _context.SaveChangesAsync();
            }
            
            return Ok(new { token, user = new {userGuid = user.guid, latestUserInfo.Email, latestUserInfo.FirstName, latestUserInfo.LastName}});
        }
        return Unauthorized();
    }
    
    //edit user
    [HttpPut("update/{userGuid}")]
    public async Task<IActionResult> EditUser([FromRoute] Guid userGuid, [FromBody] EditUserRequest request)
    {
        if (!_jwtService.ValidateToken(request.token))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.guid == userGuid);
        if (user == null)
        {
            return BadRequest("No user was found");
        }

        var latestUserInfo = await _context.UserInfo
            .Where(u => u.UserId == user.id)
            .OrderByDescending(u => u.created_at)
            .FirstOrDefaultAsync();

        // Check if the email is already taken
        if (!string.IsNullOrEmpty(request.Email))
        {
            var emailExists = await _context.LatestUserInfosView.AnyAsync(u => u.Email == request.Email);
            if (emailExists)
            {
                return BadRequest("Email is already taken");
            }
        }

        // Create a new UserInfo object with the updated information
        var newUserInfo = new UserInfo
        {
            FirstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : latestUserInfo.FirstName,
            LastName = !string.IsNullOrEmpty(request.LastName) ? request.LastName : latestUserInfo.LastName,
            Email = !string.IsNullOrEmpty(request.Email) ? request.Email : latestUserInfo.Email,
            Password = latestUserInfo.Password, // Use the password from the latest UserInfo record
            UserId = user.id,
            created_at = DateTime.UtcNow
        };

        _context.UserInfo.Add(newUserInfo);

        var numberOfChanges = await _context.SaveChangesAsync();
        if (numberOfChanges > 0)
        {
            var mergedObject = new
            {
                user = new
                {
                    guid = user.guid,
                    created_at = user.created_at
                },
                userInfo = new
                {
                    FirstName = newUserInfo.FirstName,
                    LastName = newUserInfo.LastName,
                    Email = newUserInfo.Email,
                    created_at = newUserInfo.created_at
                }
            };

            var rabbitMQService = new RabbitMQService();
            string queueName = "UserUpdateQueue";
            rabbitMQService.SendMessage(queueName,JsonSerializer.Serialize(mergedObject));
            rabbitMQService.Close();
        }
        else
        {
            return StatusCode(500, "Failed to update the database. Please check your connection and try again.");
        }

        return Ok("User was updated");
    }
    
    
    //delete user but adding a tombstone
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        if (!_jwtService.ValidateToken(request.token))
        {
            return Unauthorized();
        }
        //get the user and the latest userinfo by guid
        var user = await _context.Users.FirstOrDefaultAsync(u => u.guid == request.guid);
        if (user == null)
        {
            return BadRequest("No user was found");
        }

        var latestUserInfo = await _context.LatestUserInfosView
            .FirstOrDefaultAsync(u => u.UserId == user.id);

        //need to validate the password
        if (!hash.Verify(request.password, latestUserInfo.Password))
        {
            return Unauthorized();
        }
        
        var userTombstone = new UserTombstone
        {
            UserId = user.id,
            IsDeleted = true,
            DeletionDate = DateTime.UtcNow
        };
        
        _context.UserTombstones.Add(userTombstone);
        await _context.SaveChangesAsync(); 
        
        var rabbitMQService = new RabbitMQService();
        string queueName = "UserAnonymizeQueue";
        rabbitMQService.SendMessage(queueName,JsonSerializer.Serialize(new { user_guid = user.guid }));
        rabbitMQService.Close();

        return Ok("User was deleted");
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