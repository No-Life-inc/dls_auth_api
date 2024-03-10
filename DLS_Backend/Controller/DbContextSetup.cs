using DLS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.Controller;

public class DbContextSetup: DbContext
{
    public DbContextSetup(DbContextOptions<DbContextSetup> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
}