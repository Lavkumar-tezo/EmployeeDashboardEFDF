using System;
using System.Collections.Generic;

namespace EmployeeDirectory.DAL.Models;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string? Description { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
