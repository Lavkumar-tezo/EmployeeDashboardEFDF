using EmployeeDirectory.DAL.Models;
namespace EmployeeDirectory.DAL.Interfaces
{
    public interface IRoleRepository
    {
        public List<Role> GetRolesByDept(string deptId);
    }
}
