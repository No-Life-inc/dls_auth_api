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
    public DbSet<UserInfo> UserInfo { get; set; }
    public DbSet<LatestUserInfo> LatestUserInfosView { get; set; }
    
    public DbSet<UserTombstone> UserTombstones { get; set; }

    
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
            entity.Property(e => e.guid);

        });
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__UserInfo__3214EC0732BBF347");
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.HasOne(d => d.User)
                .WithMany(p => p.UserInfos)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserInfo_Users_UserId");
        });
        modelBuilder.Entity<LatestUserInfo>().HasNoKey();
        
        modelBuilder.Entity<UserTombstone>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.IsDeleted);
            entity.Property(e => e.DeletionDate);

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserTombstone)
                .HasForeignKey<UserTombstone>(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }
    
    public class LatestUserInfo
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime created_at { get; set; }
    }
    
    /// <summary>
    /// OnModelCreatingPartial method for the DlsUsersContext to define the model for the Users table
    /// </summary>
    /// <param name="modelBuilder"></param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
