using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.DAL.Repositories
{
    public class DepartmentRepository(LavDbEfdfContext context):IRepository<Department>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public List<Department> GetAll()
        {
            return _dbEfContext.Departments.ToList();
        }

        public Department Get(string id)
        {
            return _dbEfContext.Departments.Find(Int32.Parse(id))!;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Department department)
        {
            throw new NotImplementedException();
        }

        public void Add(Department department)
        {
            throw new NotImplementedException();
        }
    }
}
