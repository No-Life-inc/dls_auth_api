using DLS_Backend.Models;
using Microsoft.EntityFrameworkCore;


namespace DLS_Backend.Controller;

public partial class DlsUsersContext : DbContext
{
    /// <summary>
    /// Constructor for the DlsUsersContext with options
    /// </summary>
    /// <param name="options"></param>
    public DlsUsersContext(DbContextOptions<DlsUsersContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Users table
    /// </summary>
    public virtual DbSet<User> Users { get; set; }

    /// <summary>
    /// OnModelCreating method for the DlsUsersContext to define the model for the Users table
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Users__3214EC0732BBF347");
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.guid);
            entity.Property(e => e.first_name).HasMaxLength(255);
            entity.Property(e => e.last_name).HasMaxLength(255);
            entity.Property(e => e.password).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    /// <summary>
    /// OnModelCreatingPartial method for the DlsUsersContext to define the model for the Users table
    /// </summary>
    /// <param name="modelBuilder"></param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
