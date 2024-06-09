using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Exceptions;

namespace EmployeeDirectory.DAL.Repositories
{
    public class RoleRepository(LavDbEfdfContext context):IRepository<Role>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public async Task<List<Role>> GetAll()
        {
            List<Role> roles = await _dbEfContext.Roles.Include(r=> r.Departments).Include(r=> r.Locations).ToListAsync();
            return roles;
        }

        public async Task Add(Role newRole)
        {
            await _dbEfContext.Roles.AddAsync(newRole);
            _dbEfContext.SaveChanges();
        }
        public async Task Delete(string roleId)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> Get(string roleId)
        {
            List<Role>? roles = await GetAll();
            Role? role = roles.FirstOrDefault(role => role.Id.ToLower() == roleId.ToLower());
            if (role != null)
            {
                return role;
            }
            throw new Exception("Role not found");
        }

        public async Task Update(Role role)
        {
            throw new NotImplementedException();
        }

    }
}
