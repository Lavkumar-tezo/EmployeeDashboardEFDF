using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.BAL.Interfaces.Providers;

namespace EmployeeDirectory.BAL.Providers
{
    public class RoleProvider(IRepository<Role> data, IProvider<Department> dept, IProvider<Location> loc):IRoleProvider
    {
        private readonly IRepository<Role> _role = data;
        private readonly IProvider<Department> _dept = dept;
        private readonly IProvider<Location> _loc =loc;

        public async Task AddRole(Dictionary<string, string> inputs)
        {
            Role role = new()
            {
                Name = inputs["Name"],
                Description = inputs["Description"],
                Id =await GenerateRoleId()
            };
            List<Department> departments =await _dept.GetList();
            List<string> deptIds = new List<string>(inputs["Department"].Split(','));
            foreach (string id in deptIds)
            {
                role.Departments.Add(departments.First(x => x.Id.ToString().Equals(id)));
            }
            List<Location> locations =await _loc.GetList();
            List<string> locIds = new List<string>(inputs["Location"].Split(','));
            foreach(string id in locIds)
            {
                role.Locations.Add(locations.First(x=> x.Id.ToString().Equals(id)));
            }
            await _role.Add(role);
        }

        private async Task<string> GenerateRoleId()
        {
            List<Role> roles =await _role.GetAll();
            if (roles.Count == 0)
            {
                return "IN001";
            }
            string LastRoleId = roles[^1].Id ?? "";
            int lastRoleNumber = int.Parse(LastRoleId[2..]);
            lastRoleNumber++;
            string newId = "IN" + lastRoleNumber.ToString("D3");
            return newId;
        }

        public async Task<List<Role>> GetRoles()
        {
            List<Role> roles=await _role.GetAll();
            return roles;
        }

        public async Task<Dictionary<string, string>> GetIdName()
        {
            List<Role> roles =await GetRoles();
            Dictionary<string, string> roleIdName = new Dictionary<string, string>();
            foreach (Role r in roles)
            {
                roleIdName.Add(r.Id.ToString(), r.Name);
            }
            return roleIdName;
        }

        public async Task<Role> GetRole(string id)
        {
            Role role=await _role.Get(id);
            return role;
        }

    }
}
