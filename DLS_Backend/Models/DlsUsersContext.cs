﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.Models;

public partial class DlsUsersContext : DbContext
{
    public DlsUsersContext()
    {
    }

    public DlsUsersContext(DbContextOptions<DlsUsersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=DLS_Users;User Id=Developer;Password=Dev123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Users__3214EC0732BBF347");
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.guid).HasMaxLength(255);
            entity.Property(e => e.first_name).HasMaxLength(255);
            entity.Property(e => e.last_name).HasMaxLength(255);
            entity.Property(e => e.password).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}