using System;
using System.Collections.Generic;

namespace EmployeeDirectory.DAL.Models;

public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
