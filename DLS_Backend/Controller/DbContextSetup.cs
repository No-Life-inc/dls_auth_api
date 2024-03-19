using DLS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.Controller;

public class DbContextSetup: DbContext
{
    /// <summary>
    /// Constructor for the DbContextSetup
    /// </summary>
    /// <param name="options"></param>
    public DbContextSetup(DbContextOptions<DbContextSetup> options)
        : base(options)
    {
    }
    /// <summary>
    /// DbSet for the User model
    /// </summary>
    public DbSet<User> Users { get; set; }
}