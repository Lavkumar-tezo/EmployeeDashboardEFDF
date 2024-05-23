using System.Globalization;
using EmployeeDirectory.DAL.Exceptions;
using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Extension;
using Employee = EmployeeDirectory.DAL.Models.Employee;
using EmployeeDirectory.DAL.Models;
using Microsoft.IdentityModel.Tokens;
using EmployeeDirectory.DAL.Interfaces;

namespace EmployeeDirectory.BAL.Providers
{
    public class EmployeeProvider(IGetProperty prop, IGenericRepository<Employee> employee,IRoleProvider role, IGenericRepository<Department> dept, IGenericRepository<Project> proj) : IEmpProvider
    {
        private readonly IGetProperty _getProperty = prop;
        private int _employeeIndex;
        private string _employeeId = "";
        private readonly IGenericRepository<Employee> _employee =employee;
        private readonly IRoleProvider _role = role;
        private readonly IGenericRepository<Department> _dept = dept;
        private readonly IGenericRepository<Project> _proj = proj;

        public DTO.Employee AddValueToEmp(Dictionary<string, string> values, string mode)
        {
            if (string.Equals("Add",mode))
            {
                DTO.Employee newEmp = new()
                {
                    FirstName = values["FirstName"],
                    LastName = values["LastName"],
                    Email = values["Email"],
                    JoinDate = DateTime.Parse(values["JoinDate"]),
                    Location = values["Location"],
                    Role = values["Role"],
                    Manager = values["Manager"],
                    Mobile = values["Mobile"],
                    Department= Int32.Parse(values["Department"]),
                    DOB = (values["DOB"] != null) ? DateTime.Parse(values["DOB"]) : DateTime.Now
                };
                if (!values["Project"].IsNullOrEmpty())
                {
                    newEmp.Project = Int32.Parse(values["Project"]);
                }
                return newEmp;
            }
            else
            {
                (bool check,Employee emp) = GetEmployeeById(values["Id"]);
                DTO.Employee updateEmp = new()
                {
                    FirstName = (values["FirstName"].IsEmpty()) ? emp.FirstName : values["FirstName"],
                    LastName = (values["LastName"].IsEmpty()) ? emp.LastName : values["LastName"],
                    Location = (values["Location"].IsEmpty()) ? emp.Location : values["Location"],
                    Email = (values["Email"].IsEmpty()) ? emp.Email : values["Email"],
                    Role = (values["Role"].IsEmpty()) ? emp.RoleId : values["Role"],
                    Manager = (values["Manager"].IsEmpty()) ? emp.Manager : values["Manager"],
                    Project = (values["Project"].IsEmpty()) ? emp.Project?.Id : Int32.Parse(values["Project"]),
                    JoinDate = (values["JoinDate"].IsEmpty()) ? emp.JoiningDate : DateTime.Parse(values["JoinDate"]),
                    Department = (values["Department"].IsEmpty()) ? emp.Department.Id : Int32.Parse(values["Department"]),
                    Mobile = (values["Mobile"]).IsEmpty()? emp.Mobile : values["Mobile"],
                    DOB=(values["DOB"].IsEmpty())? emp.Dob:DateTime.Parse(values["DOB"])
                };
                if (!values["DOB"].IsEmpty())
                {
                    updateEmp.DOB = DateTime.Parse(values["DOB"]);
                }
                else if(!emp.Dob.HasValue)
                {
                    updateEmp.DOB = DateTime.Now;
                }
                else
                {
                    updateEmp.DOB = emp.Dob;
                }
                return updateEmp;
            }
        }

        private Employee AssignValueToModel(DTO.Employee emp)
        {
            if (_employeeId.IsEmpty())
            {
                _employeeId = GenerateEmpId();
            }
            Employee newEmp = new()
            {
                Id = _employeeId,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Location = emp.Location,
                Manager = emp.Manager,
                JoiningDate = emp.JoinDate,
                Dob = emp.DOB,
                Email = emp.Email,
                RoleId = emp.Role,
                Mobile = emp.Mobile,
            };
            if (emp.Project != null)
            {
                newEmp.Project = _proj.Get(emp.Project.ToString()!);
                newEmp.ProjectId = emp.Project;
            }
            newEmp.Department = _dept.Get(emp.Department.ToString()!);
            newEmp.DepartmentId=newEmp.Department.Id;
            newEmp.Role=_role.GetRole(emp.Role.ToString());
            return newEmp;
        }

        public void AddEmployee(DTO.Employee employee)
        {
            _employee.Add(AssignValueToModel(employee));
        }

        public List<Employee> GetEmployees()
        {
            return _employee.GetAll();
        }

        public (bool, Employee) GetEmployeeById(string id)
        {
            id = id.ToUpper();
            List<Employee> employeeList = GetEmployees();
            _employeeIndex = employeeList.FindIndex(emp => emp.Id == id);
            if (_employeeIndex != -1)
            {
                _employeeId = employeeList[_employeeIndex].Id;
                ModelKeyStore.deptId= employeeList[_employeeIndex].DepartmentId.ToString()!;
                return (true, employeeList[_employeeIndex]);
            }
            throw new EmpNotFound("Employee Not found with given Id");

        }

        public bool IsEmployeePresent(string id)
        {
            (bool check, Employee employee) = GetEmployeeById(id);
            MessagesInputStore.inputFieldValues = _getProperty.GetValueFromObject(employee);
            return check;
        }

        public void UpdateEmployee(DTO.Employee employee)
        {
            _employee.Update(AssignValueToModel(employee));

        }

        public void DeleteEmployee(string id)
        {
            _employee.Delete(id);

        }

        public string GenerateEmpId()
        {
            List<Employee> employees = _employee.GetAll();
            if (employees.Count == 0)
            {
                return "TZ0001";
            }
            Employee employeeWithMaxId = employees.OrderByDescending(e => e.Id).FirstOrDefault()!;
            string LastEmpId = employeeWithMaxId.Id;
            int lastEmpNumber = int.Parse(LastEmpId[2..]);
            lastEmpNumber++;
            string newId = "TZ" + lastEmpNumber.ToString("D4");
            return newId;
        }

    }
}
