using EmployeeDirectory.DAL.Models;
namespace EmployeeDirectory.BAL.Interfaces
{
    public interface IRoleProvider
    {
        public void AddRole(Dictionary<string, string> inputs);

        public List<Role> GetRoles();

        public Role GetRole(string id);

        public List<Role> GetRolesByDept(string deptId);
    }
}
