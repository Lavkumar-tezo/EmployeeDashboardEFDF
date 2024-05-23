using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Providers
{
    public class DepartmentProvider(IGenericRepository<Department> dept):IDepartmentProvider
    {
        private readonly IGenericRepository<Department> _dept=dept;

        public List<Department> GetList()
        {
            return _dept.GetAll();
        }
        public Dictionary<string,string> GetDepartments()
        {
            List<Department> departments = GetList();
            Dictionary<string, string> deptList = new Dictionary<string, string>();
            foreach (Department d in departments)
            {
                deptList.Add(d.Id.ToString(), d.Name);
            }
            return deptList;
        }
    }
}
