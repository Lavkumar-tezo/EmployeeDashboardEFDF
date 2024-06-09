using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

public class DepartmentProvider(IRepository<Department> dept) : IProvider<Department>
{
    private readonly IRepository<Department> _dept = dept;

    public async Task<List<Department>> GetList()
    {
        List<Department> departments= await _dept.GetAll();
        return departments;
    }

    public async Task<Department> Get(string id)
    {
        int deptId = Int32.Parse(id);
        List<Department> list = await GetList();
        Department dept = list.First(x => x.Id == deptId);
        return dept;
    }

    public async Task<Dictionary<string, string>> GetIdName()
    {
        List<Department> projects =await GetList();
        Dictionary<string, string> deptList = new Dictionary<string, string>();
        foreach (Department d in projects)
        {
            deptList.Add(d.Id.ToString(), d.Name);
        }
        return deptList;
    }
}