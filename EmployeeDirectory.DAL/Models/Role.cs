using System;
using System.Collections.Generic;

namespace EmployeeDirectory.DAL.Models;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
