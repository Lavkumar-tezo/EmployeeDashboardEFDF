using EmployeeDirectory.BAL.Extension;
using Employee = EmployeeDirectory.DAL.Models.Employee;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.BAL.Interfaces.Providers;
using System.Runtime.InteropServices;

namespace EmployeeDirectory.BAL.Providers
{
    public class EmployeeProvider(IRepository<Employee> employee, IRoleProvider role, IProvider<Location> loc, IProvider<Department> dept, IProvider<Project> proj): IEmployeeProvider
    {
        private readonly IRepository<Employee> _employee =employee;
        private readonly IRoleProvider _role = role;
        private readonly IProvider<Location> _loc=loc;
        private readonly IProvider<Department> _dept = dept;
        private readonly IProvider<Project> _proj = proj;

        public DTO.Employee AddValueToDTO(Dictionary<string, string> values, string mode)
        {
           if (string.Equals("Add",mode))
           {
                DTO.Employee newEmp = new();
                newEmp.FirstName = values["FirstName"];
                newEmp.LastName = values["LastName"];
                newEmp.Email = values["Email"];
                if (DateTime.TryParseExact(values["JoinDate"], "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedJoinDate))
                {
                    newEmp.JoinDate = parsedJoinDate;
                }
                newEmp.JoinDate = DateTime.Parse(values["JoinDate"]);
                newEmp.Location = Int32.Parse(values["Location"]);
                newEmp.Role = values["Role"];
                newEmp.Manager = values["Manager"];
                newEmp.Mobile = values["Mobile"];
                newEmp.Department = Int32.Parse(values["Department"]);
                if (DateTime.TryParseExact(values["DOB"], "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDOB))
                {
                    newEmp.DOB = parsedDOB;
                }
                if (!values["Project"].IsEmpty())
               {
                   newEmp.Project = Int32.Parse(values["Project"]);
               }
               return newEmp;
           }
           else
           {
               (bool check,Employee emp) = GetEmployeeById(SelectedEmployee.Id);
               DTO.Employee updateEmp = new()
               {
                   FirstName = (values["FirstName"].IsEmpty()) ? emp.FirstName : values["FirstName"],
                   LastName = (values["LastName"].IsEmpty()) ? emp.LastName : values["LastName"],
                   Location = (values["Location"].IsEmpty()) ? emp.LocationId : Int32.Parse(values["Location"]),
                   Email = (values["Email"].IsEmpty()) ? emp.Email : values["Email"],
                   Role = (values["Role"].IsEmpty()) ? emp.RoleId : values["Role"],
                   Manager = (values["Manager"].IsEmpty()) ? emp.ManagerId : values["Manager"],
                   Project = (values["Project"].IsEmpty()) ? emp.Project?.Id : Int32.Parse(values["Project"]),
                   JoinDate = emp.JoiningDate,
                   Department = (values["Department"].IsEmpty()) ? emp.Department.Id : Int32.Parse(values["Department"]),
                   Mobile = (values["Mobile"]).IsEmpty()? emp.Mobile : values["Mobile"],
                   DOB=emp.Dob
               };
                if (DateTime.TryParseExact(values["JoinDate"], "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedJoinDate))
                {
                    updateEmp.JoinDate = parsedJoinDate;
                }
                if (DateTime.TryParseExact(values["DOB"], "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDOB))
                {
                    updateEmp.DOB = parsedDOB;
                }
                return updateEmp;
           }
        }

        private Employee AssignValueToModel(DTO.Employee emp,string mode,[Optional] Employee selectedEmp)
        {
           Employee newEmp= new Employee();
            if(selectedEmp!=null)
            {
                newEmp = selectedEmp;
            }
           if (string.Equals(mode,"Add"))
           {
               newEmp.Id = GenerateEmpId();
           }
            else
            {
                newEmp.Id = SelectedEmployee.Id;
            }
            newEmp.FirstName = emp.FirstName;
            newEmp.LastName = emp.LastName;
            newEmp.LocationId = emp.Location;
            newEmp.JoiningDate = emp.JoinDate;
            newEmp.Dob = emp.DOB;
            newEmp.Email = emp.Email;
            newEmp.Mobile = emp.Mobile;
           if (emp.Project != null)
           {
               newEmp.Project = _proj.Get(emp.Project.ToString()!);
               newEmp.ProjectId = newEmp.Project.Id;
           }
           if(emp.Manager != null)
            {
                newEmp.Manager=_employee.Get(emp.Manager);
                newEmp.ManagerId = emp.Manager;
            }
           newEmp.Department = _dept.Get(emp.Department.ToString()!);
           newEmp.DepartmentId=newEmp.Department.Id;
           newEmp.Role=_role.GetRole(emp.Role.ToString());
           newEmp.RoleId=newEmp.Role.Id;
           newEmp.Location = _loc.Get(emp.Location.ToString()!);
           newEmp.LocationId=newEmp.Location.Id;
           return newEmp;
        }

        public void AddEmployee(DTO.Employee employee)
        {
           _employee.Add(AssignValueToModel(employee,"Add"));
        }

        public List<Employee> GetEmployees()
        {
           return _employee.GetAll();
        }

        public List<Employee> GetManagers()
        {
            List<Employee> list = GetEmployees().Where(x=> x.IsManager==true).ToList();
            return list;
        }

        public (bool, Employee) GetEmployeeById(string id)
        {
            id = id.ToUpper();
            Employee emp = _employee.Get(id);
            SelectedEmployee.Id = emp.Id;
            SelectedEmployee.deptName = emp.Department.Name;
            SelectedEmployee.locName = emp.Location.Name;
            SelectedEmployee.roleName = emp.Role.Name;
            return (true, emp);

        }

        public void UpdateEmployee(DTO.Employee employee, Employee emp)
        {
           _employee.Update(AssignValueToModel(employee,"Edit",emp));

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
