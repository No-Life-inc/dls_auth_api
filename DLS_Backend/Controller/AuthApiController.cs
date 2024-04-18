﻿using System.Text.Json;
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
        var hashedPassword = hash.Hash(request.password); // Hash the password before storing
        
        var userCheck = await _context.Users.FirstOrDefaultAsync(u => u.email == request.email);
        if (userCheck != null)
        {
            return BadRequest("User already exists");
        }
        
        var user = new User
        {
            first_name = request.first_name,
            last_name = request.last_name,
            email = request.email,
            password = hashedPassword, // Store the hashed password
            guid = request.guid, 
            created_at = DateTime.UtcNow // Set the created time
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Save changes in the DB context
    
        user.password = null; // Remove the password from the user object
    
        string userJson = JsonSerializer.Serialize(user);
    
        var rabbitMQService = new RabbitMQService();
        rabbitMQService.SendMessage(userJson);
        rabbitMQService.Close();

        return CreatedAtRoute("GetUser", new { email = user.email }, user);
    }
    
    /// <summary>
    ///  Login method for the AuthApiController
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.email == request.email);
        if (user == null)
        {
            return (IActionResult)Results.NotFound("User not found");
        }
        if (hash.Verify(request.password, user.password))
        {
            var token = _jwtService.GenerateToken(user.guid);
            return Ok(new { token });
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