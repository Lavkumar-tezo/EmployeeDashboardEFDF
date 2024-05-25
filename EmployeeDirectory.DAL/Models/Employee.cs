using System;
using System.Collections.Generic;

namespace EmployeeDirectory.DAL.Models;

public partial class Employee
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime JoiningDate { get; set; }

    public int LocationId { get; set; }

    public int DepartmentId { get; set; }

    public string RoleId { get; set; } = null!;

    public int? ProjectId { get; set; }

    public string? ManagerId { get; set; }

    public DateTime? Dob { get; set; }

    public string? Mobile { get; set; }

    public bool? IsManager { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual Location Location { get; set; } = null!;

    public virtual Employee? Manager { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Role Role { get; set; } = null!;
}
