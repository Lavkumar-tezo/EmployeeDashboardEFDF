using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeDirectory.DAL.Interfaces;

namespace EmployeeDirectory.DAL.Repositories
{
    public class RoleRepository(LavDbEfContext context) : IGenericRepository<Role>, IRoleRepository
    {
        private readonly LavDbEfContext _dbEfContext = context;

        public List<Role> GetAll()
        {
            List<Role> roles = _dbEfContext.Roles.Include("Department").ToList();
            return roles;
        }

        public void Add(Role newRole)
        {
            _dbEfContext.Roles.Add(newRole);
            _dbEfContext.SaveChanges();
        }
        public void Delete(string roleId)
        {
            throw new NotImplementedException();
        }

        public Role Get(string roleId)
        {
            return _dbEfContext.Roles.Find(roleId)!;
        }

        public void Update(Role role)
        {
            throw new NotImplementedException();
        }

        public List<Role> GetRolesByDept(string deptId)
        {
            List<Role> list = _dbEfContext.Roles.Include("Department").Where(role => role.Department.Id.ToString() == deptId).ToList();
            return list;
        }
    }
}
