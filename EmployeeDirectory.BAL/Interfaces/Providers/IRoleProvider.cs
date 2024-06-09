using EmployeeDirectory.DAL.Models;
namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IRoleProvider
    {
        public Task AddRole(Dictionary<string, string> inputs);

        public Task<List<Role>> GetRoles();

        public Task<Role> GetRole(string id);

        public Task<Dictionary<string, string>> GetIdName();


    }
}
