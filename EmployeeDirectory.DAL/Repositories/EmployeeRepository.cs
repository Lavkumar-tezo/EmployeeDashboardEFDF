using EmployeeDirectory.DAL.Exceptions;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectory.DAL.Repositories
{
    public class EmployeeRepository(LavDbEfdfContext context):IRepository<Employee>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public async  Task<List<Employee>> GetAll()
        {

           List<Employee> employees =await _dbEfContext.Employees.Where(x => x.IsDeleted !=true).Include("Department").Include("Project").Include("Role").Include("Location").ToListAsync();
           return employees;
        }

        public async Task<Employee> Get(string empId)
        {
           List<Employee>? employees = await GetAll();
           Employee? employee = employees.FirstOrDefault(e => e.Id.ToLower() == empId.ToLower());
           if (employee != null)
           {
               return employee;
           }
           throw new EmpNotFound("Employee with given id not found");
        }

        public async Task Add(Employee newEmp)
        {
           await _dbEfContext.Employees.AddAsync(newEmp);
           _dbEfContext.SaveChanges();
        }

        public async Task Update(Employee updatedEmp)
        {
           _dbEfContext.Entry(updatedEmp).State =EntityState.Modified;
           await _dbEfContext.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
           Employee? emp =await _dbEfContext.Employees.FirstOrDefaultAsync(emp => emp.Id == id)!;
           if (emp != null)
           {
               emp.IsDeleted=true;
                _dbEfContext.SaveChanges();
                return;
           }
            throw new EmpNotFound("Selected Employee Not found");
           
        }

    }
}
