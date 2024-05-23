using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.DAL.Interfaces;

namespace EmployeeDirectory.BAL.Providers
{
    public class RoleProvider(IGenericRepository<Role> data,IGenericRepository<Department> dept,IRoleRepository role) : IRoleProvider
    {
        private readonly IGenericRepository<Role> _role = data;
        private readonly IGenericRepository<Department> _dept =dept;
        private readonly IRoleRepository _roleRepository =role;

        public  void AddRole(Dictionary<string, string> inputs)
        {
            Role role = new()
            {
                Name = inputs["Name"],
                Location = inputs["Location"],
                Description = inputs["Description"],
                Id = GenerateRoleId()
            };           
            List<Department> departments =  _dept.GetAll();
            role.Department = departments.First(x => x.Id.ToString().Equals(inputs["Department"]))!;
            role.DepartmentId=role.Department.Id;
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

        public Role GetRole(string id)
        {
            return _role.Get(id);
        }

        public List<Role> GetRolesByDept(string deptId)
        {
            return _roleRepository.GetRolesByDept(deptId);
        }
    }
}
