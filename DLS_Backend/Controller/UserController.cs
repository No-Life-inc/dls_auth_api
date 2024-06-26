﻿/*using DLS_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.Controller;

public class UserController : ControllerBase
{
    private readonly DlsUsersContext _context;
    private Hashing _hashing;


    /// <summary>
    /// Constructor for the UserController
    /// </summary>
    /// <param name="context"></param>
    public UserController(DlsUsersContext context)
    {
        _context = context;
        _hashing = new Hashing();
    }
    
    /// <summary>
    /// Creates a new user in the database
    /// </summary>
    /// <param name="userInput"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User userInput)
    {
        var newUser = new User
        {
            first_name = userInput.first_name,
            last_name = userInput.last_name,
            password = _hashing.Hash(userInput.password), 
            email = userInput.email
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = newUser.id }, newUser);
    }
    
    /// <summary>
    /// Gets a user from the database by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet("v1/get-user/{email}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.email == email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}*/