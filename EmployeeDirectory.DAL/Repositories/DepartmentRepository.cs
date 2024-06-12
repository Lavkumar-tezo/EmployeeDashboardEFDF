using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectory.DAL.Repositories
{
    public class DepartmentRepository(LavDbEfdfContext context):IRepository<Department>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public async  Task<List<Department>> GetAll()
        {
            List<Department> departments = await _dbEfContext.Departments.ToListAsync();
            return departments;
        }

        public async Task<Department> Get(string id)
        {
            List<Department> departments=await GetAll();
            Department? department=departments.FirstOrDefault(dept=>string.Equals(dept.Id, id));
            if (department != null)
            {
                return department;
            }
            throw new Exception("Selected Department Not found");
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Department department)
        {
            throw new NotImplementedException();
        }

        public async Task Add(Department department)
        {
            throw new NotImplementedException();
        }
    }
}
