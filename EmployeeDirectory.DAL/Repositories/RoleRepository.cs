using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeDirectory.DAL.Interfaces;

namespace EmployeeDirectory.DAL.Repositories
{
    public class RoleRepository(LavDbEfdfContext context):IRepository<Role>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public List<Role> GetAll()
        {
            List<Role> roles = _dbEfContext.Roles.Include(r=> r.Departments).Include(r=> r.Locations).ToList();
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

    }
}
