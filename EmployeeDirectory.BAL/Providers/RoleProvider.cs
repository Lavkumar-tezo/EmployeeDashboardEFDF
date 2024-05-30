using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.DAL.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using EmployeeDirectory.BAL.Interfaces.Providers;

namespace EmployeeDirectory.BAL.Providers
{
    public class RoleProvider(IRepository<Role> data, IProvider<Department> dept, IProvider<Location> loc):IRoleProvider
    {
        private readonly IRepository<Role> _role = data;
        private readonly IProvider<Department> _dept = dept;
        private readonly IProvider<Location> _loc =loc;

        public void AddRole(Dictionary<string, string> inputs)
        {
            Role role = new()
            {
                Name = inputs["Name"],
                Description = inputs["Description"],
                Id = GenerateRoleId()
            };
            List<Department> departments = _dept.GetList();
            List<string> deptIds = new List<string>(inputs["Department"].Split(','));
            foreach (string id in deptIds)
            {
                role.Departments.Add(departments.First(x => x.Id.ToString().Equals(id)));
            }
            List<Location> locations = _loc.GetList();
            List<string> locIds = new List<string>(inputs["Location"].Split(','));
            foreach(string id in locIds)
            {
                role.Locations.Add(locations.First(x=> x.Id.ToString().Equals(id)));
            }
            _role.Add(role);
        }

        public string GenerateRoleId()
        {
            List<Role> roles = _role.GetAll();
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

        public List<Role> GetRoles()
        {
            return _role.GetAll();
        }

        public Dictionary<string, string> GetIdName()
        {
            List<Role> roles = GetRoles();
            Dictionary<string, string> roleIdName = new Dictionary<string, string>();
            foreach (Role r in roles)
            {
                roleIdName.Add(r.Id.ToString(), r.Name);
            }
            return roleIdName;
        }

        public Role GetRole(string id)
        {
            return _role.Get(id);
        }

    }
}
