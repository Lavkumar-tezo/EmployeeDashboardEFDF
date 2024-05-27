using Employee = EmployeeDirectory.DAL.Models.Employee;
namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IEmployeeProvider
    {
        public void AddEmployee(DTO.Employee employee);

        public List<Employee> GetEmployees();

        public List<Employee> GetManagers();

        public (bool, Employee) GetEmployeeById(string id);

        public void UpdateEmployee(DTO.Employee employee, Employee emp);

        public void DeleteEmployee(string id);

        public DTO.Employee AddValueToDTO(Dictionary<string, string> values, string mode);
    }
}
