using System;
using System.Collections.Generic;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeDirectory.DAL.Repositories;

public partial class LavDbEfdfContext : DbContext
{
    public LavDbEfdfContext()
    {
    }

    public LavDbEfdfContext(DbContextOptions<LavDbEfdfContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("app-settings.json").Build();
        string connectionString = configBuilder["connection:sql"]!;
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC276E82FC4B");

            entity.ToTable("Department");

            entity.HasIndex(e => e.Name, "UQ__Departme__737584F62DC76CED").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(25);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC2793B547C7");

            entity.ToTable("Employee");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .HasColumnName("ID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(35);
            entity.Property(e => e.IsManager).HasDefaultValue(false);
            entity.Property(e => e.JoiningDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(35);
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.ManagerId)
                .HasMaxLength(6)
                .HasColumnName("ManagerID");
            entity.Property(e => e.Mobile).HasMaxLength(10);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.RoleId)
                .HasMaxLength(5)
                .HasColumnName("RoleID");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__Depart__1BC821DD");

            entity.HasOne(d => d.Location).WithMany(p => p.Employees)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__Locati__1AD3FDA4");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Employee__Manage__1EA48E88");

            entity.HasOne(d => d.Project).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Employee__Projec__1DB06A4F");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__RoleID__1CBC4616");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC27BD7A00C5");

            entity.ToTable("Location");

            entity.HasIndex(e => e.Name, "UQ__Location__737584F6E81E04F9").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3214EC2769CE4BEC");

            entity.ToTable("Project");

            entity.HasIndex(e => e.Name, "UQ__Project__737584F623A962D0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(35);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC276DC835C3");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .HasMaxLength(5)
                .HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(25);

            entity.HasMany(d => d.Departments).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleDepartment",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .HasConstraintName("FK__RoleDepar__Depar__17F790F9"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__RoleDepar__RoleI__17036CC0"),
                    j =>
                    {
                        j.HasKey("RoleId", "DepartmentId").HasName("PK__RoleDepa__41DAB78618B1DC29");
                        j.ToTable("RoleDepartment");
                        j.IndexerProperty<string>("RoleId")
                            .HasMaxLength(5)
                            .HasColumnName("RoleID");
                        j.IndexerProperty<int>("DepartmentId").HasColumnName("DepartmentID");
                    });

            entity.HasMany(d => d.Locations).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleLocation",
                    r => r.HasOne<Location>().WithMany()
                        .HasForeignKey("LocationId")
                        .HasConstraintName("FK__RoleLocat__Locat__14270015"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__RoleLocat__RoleI__1332DBDC"),
                    j =>
                    {
                        j.HasKey("RoleId", "LocationId").HasName("PK__RoleLoca__E485247DC8C896EC");
                        j.ToTable("RoleLocation");
                        j.IndexerProperty<string>("RoleId")
                            .HasMaxLength(5)
                            .HasColumnName("RoleID");
                        j.IndexerProperty<int>("LocationId").HasColumnName("LocationID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
