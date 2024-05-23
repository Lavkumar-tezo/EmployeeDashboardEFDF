using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeDirectory.DAL.Repositories;

public partial class LavDbEfContext : DbContext
{
    public LavDbEfContext()
    {
    }

    public LavDbEfContext(DbContextOptions<LavDbEfContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

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
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC270F36F2F1");

            entity.ToTable("Department");

            entity.HasIndex(e => e.Name, "UQ__Departme__737584F622FD2CAC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(25);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC2793F65B9E");

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
            entity.Property(e => e.JoiningDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(35);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Manager).HasMaxLength(70);
            entity.Property(e => e.Mobile).HasMaxLength(10);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.RoleId)
                .HasMaxLength(5)
                .HasColumnName("RoleID");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__Depart__4AB81AF0");

            entity.HasOne(d => d.Project).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Employee__Projec__4CA06362");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__RoleID__4BAC3F29");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3214EC2799F30ECA");

            entity.ToTable("Project");

            entity.HasIndex(e => e.Name, "UQ__Project__737584F67BC6BDAC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(35);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC27524F95DA");

            entity.ToTable("Role");

            entity.HasIndex(e => new { e.Name, e.DepartmentId }, "UQ__Role__B855FD4A14C93C64").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(5)
                .HasColumnName("ID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(25);

            entity.HasOne(d => d.Department).WithMany(p => p.Roles)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Role__Department__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
