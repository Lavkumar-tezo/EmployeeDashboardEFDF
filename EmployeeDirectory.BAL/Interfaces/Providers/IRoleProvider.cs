using EmployeeDirectory.DAL.Models;
namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IRoleProvider
    {
        public void AddRole(Dictionary<string, string> inputs);

        public List<Role> GetRoles();

        public Role GetRole(string id);

        public Dictionary<string, string> GetIdName();


    }
}
