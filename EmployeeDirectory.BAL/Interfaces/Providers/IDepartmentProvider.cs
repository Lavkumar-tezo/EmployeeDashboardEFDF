using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IDepartmentProvider
    {
        public List<Department> GetList();

        public Dictionary<string, string> GetDepartments();
    }
}
