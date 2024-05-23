namespace EmployeeDirectory.DAL.Models;

public partial class Employee
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime JoiningDate { get; set; }

    public string Location { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string RoleId { get; set; } = null!;

    public int? ProjectId { get; set; }

    public string? Manager { get; set; }

    public DateTime? Dob { get; set; }

    public string? Mobile { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual Role Role { get; set; } = null!;
}
