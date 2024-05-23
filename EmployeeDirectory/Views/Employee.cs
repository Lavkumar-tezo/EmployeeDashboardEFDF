using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.Helpers;
using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.BAL.Interfaces.Views;
using EmployeeDirectory.DAL.Models;
namespace EmployeeDirectory.Views
{
    internal class Employee(IValidator validator,IRoleProvider role, IEmpProvider emp, IGetProperty prop,IDepartmentProvider dept, IProjectProvider proj) : IEmployeeView
    {
        private readonly IValidator _validator = validator;
        private readonly IEmpProvider _employee = emp;
        private readonly IRoleProvider _role = role;
        private readonly IGetProperty _getProperty = prop;
        private readonly IDepartmentProvider _dept = dept;
        private readonly IProjectProvider _proj = proj;

        public void ShowEmployeeMenu()
        {
            int input;

        EmployeeMenu: Printer.Print(true, "---------Employee Management Menu---------", "1. Add Employee", "2. Display All", "3. Display One", "4. Edit Employee", "5. Delete Employee", "6. Go Back");
            input = ChoiceTaker.CheckChoice(1, 6);
            switch (input)
            {
                case 1:
                    AddEmployee();
                    goto NextProcess;
                case 2:
                    DisplayEmployeeList();
                    goto NextProcess;
                case 3:
                    DisplaySelectedEmp();
                    goto NextProcess;
                case 4:
                    EditEmployee();
                    goto NextProcess;
                case 5:
                    DeleteEmployee();
                    goto NextProcess;
                case 6:
                    Printer.Print(true, "Welcome Back to Main Menu");
                    return;
            }
        NextProcess: Printer.Print(true, "Where do u want to go", "1. Go to Main Menu", "2. Go to Previous Menu");
            input = ChoiceTaker.CheckChoice(1, 2);
            switch (input)
            {
                case 1:
                    Printer.Print(true, "Welcome Back to Main Menu");
                    break;
                case 2:
                    goto EmployeeMenu;
            }
        }

        /// <summary>
        /// Takes input during adding or editing employee
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>return employee object</returns>
        public BAL.DTO.Employee TakeInput(string mode)
        {
            List<string> inputFields = _getProperty.GetProperties("Employee");
            bool isAllInputCorrect = true;
            do
            {
                Printer.Print(true, "Enter the details of employee");
                foreach (var inputName in inputFields)
                {
                    if (MessagesInputStore.validationMessages.ContainsKey(inputName) || isAllInputCorrect)
                    {
                        if (inputName.Equals("Department"))
                        {
                            List<Department> list=_dept.GetList();
                            string message = $"Available {inputName} : ";
                            foreach (var dept in list)
                            {
                                message += dept.Name + ", ";
                            }
                            message += $" -- Write full {inputName} name";
                            Printer.Print(true, message);
                        }
                        else if (inputName.Equals("Project"))
                        {
                            List<Project> list = _proj.GetList();
                            string message = $"Available {inputName} : ";
                            foreach (var project in list)
                            {
                                message += project.Name + ", ";
                            }
                            message += $" -- Write full {inputName} name";
                            Printer.Print(true, message);
                        }
                        else if (inputName.Equals("Role"))
                        {
                            List<DAL.Models.Role> list = _role.GetRoles();
                            string message = $"Role - select among these -- ";
                            foreach (var role in list)
                            {
                                message += role.Name+", ";
                            }
                            message += $" -- Write full {inputName} name";
                            Printer.Print(true, message);
                        }
                        Printer.Print(false, $"{inputName} : ");
                        MessagesInputStore.inputFieldValues[inputName] = Console.ReadLine() ?? "";
                    }
                }
                isAllInputCorrect = _validator.ValidateEmployeeInputs(mode, ref isAllInputCorrect);
                if (!isAllInputCorrect)
                {
                    foreach (var item in MessagesInputStore.validationMessages)
                    {
                        Printer.Print(true, $"{item.Value}");
                    }
                }
            } while (!isAllInputCorrect);
            MessagesInputStore.validationMessages.Clear();
            BAL.DTO.Employee emp = _employee.AddValueToEmp(MessagesInputStore.inputFieldValues, mode);
            MessagesInputStore.inputFieldValues.Clear();
            return emp;
        }

        public void AddEmployee()
        {
            try
            {
                List<DAL.Models.Role> roleList = _role.GetRoles();
                if (roleList.Count == 0)
                {
                    Printer.Print(true, "Add a new role First");
                    return;
                }
                BAL.DTO.Employee emp = TakeInput("Add");
                _employee.AddEmployee(emp);
                Printer.Print(true, "Employee added");
            }
            catch (IOException ex)
            {
                Printer.Print(true, ex.Message);
            }

        }

        public void EditEmployee()
        {
            try
            {
                Printer.Print(false, "Enter the id of employee : ");
                string inputId = Console.ReadLine() ?? "";
                if (inputId.IsEmpty())
                {
                    throw new DAL.Exceptions.EmpNotFound("Employee Id can't be null");
                }
                bool check = _employee.IsEmployeePresent(inputId);
                BAL.DTO.Employee emp = TakeInput("Edit");
                _employee.UpdateEmployee(emp);
                Printer.Print(true, "Employee Updated");
            }
            catch (IOException ex)
            {
                Printer.Print(true, ex.Message);
            }
            catch (DAL.Exceptions.EmpNotFound ex)
            {
                Printer.Print(true, ex.Message);
            }

        }
        public void DisplayEmployeeList()
        {
            try
            {
                List<DAL.Models.Employee> employeeList = _employee.GetEmployees();
                if (employeeList.Count > 0)
                {
                    for (int i = 0; i < employeeList.Count; i++)
                    {
                        DisplayEmployee(employeeList[i]);
                    }
                }
                else
                {
                    Printer.Print(true, "No Employee Found in List");
                }
            }
            catch (IOException ex)
            {
                Printer.Print(true, ex.Message);
            }

        }

        public void DisplayEmployee(DAL.Models.Employee emp)
        {
            Printer.Print(true, $"Emp Id: {emp.Id} -- Full Name: {emp.FirstName} {emp.LastName} -- Department: {emp.Department.Name} -- Role: {emp.Role.Name} -- Email: {emp.Email} -- Location: {emp.Location} -- JoiningDate: {emp.JoiningDate} -- Manager: {emp.Manager} -- Project: {emp.Project.Name} -- DOB: {emp.Dob} -- Mobile: {emp.Mobile}");
        }

        public void DisplaySelectedEmp()
        {
            Printer.Print(false, "Enter the Id of employee: ");
            string inputId = Console.ReadLine() ?? "";
            if (inputId.IsEmpty())
            {
                Printer.Print(true, "No employee found with given id");
                return;
            }
            try
            {
                (bool check, dynamic emp) = _employee.GetEmployeeById(inputId);
                DisplayEmployee(emp);

            }
            catch (DAL.Exceptions.EmpNotFound ex)
            {
                Printer.Print(true, ex.Message);
            }
            catch (IOException ex)
            {
                Printer.Print(true, ex.Message);
            }
        }

        public void DeleteEmployee()
        {
            try
            {
                Console.Write("Enter the id of employee : ");
                string inputId = Console.ReadLine() ?? "";
                if (inputId.IsEmpty())
                {
                    Printer.Print(true, "Invalid Employee Id");
                    return;
                }
                else
                {
                    (bool check, dynamic emp) = _employee.GetEmployeeById(inputId);
                    if (check == true)
                    {
                        _employee.DeleteEmployee(inputId);
                        Printer.Print(true, "Employee deleted");
                    }
                    else
                    {
                        Printer.Print(true, emp);
                    }
                }
            }
            catch (DAL.Exceptions.EmpNotFound ex)
            {
                Printer.Print(true, ex.Message);
            }
            catch (IOException ex)
            {
                Printer.Print(true, ex.Message);
            }
        }
    }

}
