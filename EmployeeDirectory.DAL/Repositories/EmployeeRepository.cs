using EmployeeDirectory.DAL.Exceptions;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectory.DAL.Repositories
{
    public class EmployeeRepository(LavDbEfdfContext context):IGenericRepository<Employee>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public List<Employee> GetAll()
        {

           List<Employee> employees = _dbEfContext.Employees.Where(x => x.IsDeleted !=false).Include("Department").Include("Project").Include("Role").ToList();
           return employees;
        }

        public Employee Get(string empId)
        {
           List<Employee>? employees = GetAll();
           Employee? employee = employees.FirstOrDefault(e => e.Id == empId);
           if (employee != null)
           {
               return employee;
           }
           throw new EmpNotFound("Id not found");
        }

        public void Add(Employee newEmp)
        {
           _dbEfContext.Employees.Add(newEmp);
           _dbEfContext.SaveChanges();
        }

        public void Update(Employee emp)
        {
           _dbEfContext.Entry(emp).State = EntityState.Modified;
           _dbEfContext.SaveChanges();
        }

        public void Delete(string id)
        {
           Employee emp = _dbEfContext.Employees.First(emp => emp.Id == id)!;
           if (emp != null)
           {
               emp.IsDeleted=true;
           }
           _dbEfContext.SaveChanges();
        }

    }
}
