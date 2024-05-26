using System;
using System.Collections.Generic;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;

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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAV\\SQLEXPRESS;Database=Lav_DB_EFDF;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC27B60D7169");

            entity.ToTable("Department");

            entity.HasIndex(e => e.Name, "UQ__Departme__737584F605FA641F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(25);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC2755FD92BD");

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
            entity.Property(e => e.IsDeleted).HasDefaultValue(true);
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
                .HasConstraintName("FK__Employee__Depart__76969D2E");

            entity.HasOne(d => d.Location).WithMany(p => p.Employees)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__Locati__75A278F5");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Employee__Manage__797309D9");

            entity.HasOne(d => d.Project).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Employee__Projec__787EE5A0");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__RoleID__778AC167");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC27E025CAF8");

            entity.ToTable("Location");

            entity.HasIndex(e => e.Name, "UQ__Location__737584F6E16DD39F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3214EC27A5C76238");

            entity.ToTable("Project");

            entity.HasIndex(e => e.Name, "UQ__Project__737584F6813E39FF").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(35);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC273FE4BDA1");

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
                        .HasConstraintName("FK__RoleDepar__Depar__59063A47"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__RoleDepar__RoleI__5812160E"),
                    j =>
                    {
                        j.HasKey("RoleId", "DepartmentId").HasName("PK__RoleDepa__41DAB78667204AA4");
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
                        .HasConstraintName("FK__RoleLocat__Locat__5535A963"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__RoleLocat__RoleI__5441852A"),
                    j =>
                    {
                        j.HasKey("RoleId", "LocationId").HasName("PK__RoleLoca__E485247D84B71500");
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
