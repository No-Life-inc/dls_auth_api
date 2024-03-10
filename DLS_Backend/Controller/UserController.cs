using DLS_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.Controller;

public class UserController : ControllerBase
{
    private readonly DbContextSetup _context;
    private Hashing _hashing;


    public UserController(DbContextSetup context)
    {
        _context = context;
        _hashing = new Hashing();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User userInput)
    {
        var newUser = new User
        {
            Name = userInput.Name,
            Password = _hashing.Hash(userInput.Password), 
            Email = userInput.Email
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }
    
    [HttpGet("{email}")]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}