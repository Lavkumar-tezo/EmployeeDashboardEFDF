using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using System.Runtime.Intrinsics.Arm;

public class DepartmentProvider(IRepository<Department> dept) : IProvider<Department>
{
    private readonly IRepository<Department> _dept = dept;

    public List<Department> GetList()
    {
        return _dept.GetAll();
    }

    public Department Get(string id)
    {
        int deptId = Int32.Parse(id);
        List<Department> list =GetList();
        Department dept = list.First(x => x.Id == deptId);
        return dept;
    }

    public Dictionary<string, string> GetIdName()
    {
        List<Department> projects = GetList();
        Dictionary<string, string> deptList = new Dictionary<string, string>();
        foreach (Department d in projects)
        {
            deptList.Add(d.Id.ToString(), d.Name);
        }
        return deptList;
    }
}