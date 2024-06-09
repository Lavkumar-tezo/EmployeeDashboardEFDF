using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.Helpers;
using EmployeeDirectory.Interfaces.Views;
using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.BAL.Interfaces.Validators;
using EmployeeDirectory.BAL.Interfaces.Helpers;
using EmployeeDirectory.Interfaces.Helpers;
namespace EmployeeDirectory.Views
{
    internal class Employee(IEmployeeValidator empVal, IRoleProvider role, IEmployeeProvider emp, IGetProperty prop, IProvider<Department> dept, IProvider<Project> proj,IProvider<Location> loc, IChoiceTaker taker) :IEmployeeView
    {
        private readonly IEmployeeValidator _empValidator=empVal;
        private readonly IChoiceTaker _taker = taker;
        private readonly IEmployeeProvider _employee = emp;
        private readonly IRoleProvider _role = role;
        private readonly IGetProperty _getProperty = prop;
        private readonly IProvider<Department> _dept = dept;
        private readonly IProvider<Project> _proj = proj;
        private readonly IProvider<Location> _loc = loc;

        public async Task ShowEmployeeMenu()
        {
           int input;

        Printer.Print(true, "---------Employee Management Menu---------", "1. Add Employee", "2. Display All", "3. Display One", "4. Edit Employee", "5. Delete Employee", "6. Go Back");
           input = _taker.CheckChoice(1, 6);
           switch (input)
           {
               case 1:
                   await AddEmployee();
                   break;
               case 2:
                   await DisplayEmployeeList();
                   break;
               case 3:
                   await DisplaySelectedEmp();
                   break;
               case 4:
                   await EditEmployee();
                   break;
               case 5:
                   await DeleteEmployee();
                   break;
               case 6:
                   Printer.Print(true, "Welcome Back to Main Menu");
                   return;
           }
        Printer.Print(true, "Where do u want to go", "1. Go to Main Menu", "2. Go to Previous Menu");
           input = _taker.CheckChoice(1, 2);
           switch (input)
           {
               case 1:
                   Printer.Print(true, "Welcome Back to Main Menu");
                   break;
               case 2:
                    await ShowEmployeeMenu();
                    break;  
           }
        }

        /// <summary>
        /// Takes input during adding or editing employee
        /// </summary>
        /// <param name="mode"></param>
        /// <returns>return employee object</returns>
        public async Task<BAL.DTO.Employee> TakeInput(string mode)
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
                       if (inputName.Equals("Department") && isAllInputCorrect)
                       {
                           List<Department> list=await _dept.GetList();
                           string message = $"Available {inputName} : ";
                           foreach (var dept in list)
                           {
                               message += dept.Name + ", ";
                           }
                           message += $" -- Write full {inputName} name";
                           Printer.Print(true, message);
                       }
                       else if (inputName.Equals("Location") && isAllInputCorrect)
                        {
                            List<Location> list =await _loc.GetList();
                            string message = $"Available {inputName} : ";
                            foreach (var loc in list)
                            {
                                message += loc.Name + ", ";
                            }
                            message += $" -- Write full {inputName} name";
                            Printer.Print(true, message);
                        }
                        else if (inputName.Equals("Project") && isAllInputCorrect)
                       {
                           List<Project> list =await _proj.GetList();
                           string message = $"Available {inputName} : ";
                           foreach (var project in list)
                           {
                               message += project.Name + ", ";
                           }
                           message += $" -- Write full {inputName} name";
                           Printer.Print(true, message);
                       }
                       else if (inputName.Equals("Role") && isAllInputCorrect)
                       {
                           List<DAL.Models.Role> list =await _role.GetRoles();
                           string message = $"Available {inputName} : ";
                           foreach (var role in list)
                           {
                               message += role.Name+", ";
                           }
                           message += $" -- Write full {inputName} name";
                           Printer.Print(true, message);
                       }
                        else if (inputName.Equals("Manager") && isAllInputCorrect)
                        {
                            List<DAL.Models.Employee> list =await _employee.GetManagers();
                            string message = $"Managers - select id among these -- \n";
                            foreach (var employee in list)
                            {
                                message+= $"Emp Id: {employee.Id} -- Full Name: {employee.FirstName} {employee.LastName}\n";
                            }
                            message += $" -- Write id of {inputName}";
                            Printer.Print(true, message);
                        }
                        Printer.Print(false, $"{inputName} : ");
                       MessagesInputStore.inputFieldValues[inputName] = Console.ReadLine() ?? "";
                   }
               }
                isAllInputCorrect =await _empValidator.ValidateEmployeeInputs(mode, isAllInputCorrect);
               if (!isAllInputCorrect)
               {
                   foreach (var item in MessagesInputStore.validationMessages)
                   {
                       Printer.Print(true, $"{item.Value}");
                   }
               }
           } while (!isAllInputCorrect);
           MessagesInputStore.validationMessages.Clear();
           BAL.DTO.Employee emp =await _employee.AddValueToDTO(MessagesInputStore.inputFieldValues, mode);
           MessagesInputStore.inputFieldValues.Clear();
           return emp;
        }

        public async Task AddEmployee()
        {
           try
           {
               List<DAL.Models.Role> roleList =await _role.GetRoles();
               if (roleList.Count == 0)
               {
                   Printer.Print(true, "Add a new role First");
                   return;
               }
               BAL.DTO.Employee emp =await TakeInput("Add");
               await _employee.AddEmployee(emp);
               Printer.Print(true, "Employee added");
           }
           catch (Exception ex)
           {
               Printer.Print(true, ex.Message);
           }

        }

        public async Task EditEmployee()
        {
           try
           {
               Printer.Print(false, "Enter the id of employee : ");
               string inputId = Console.ReadLine() ?? "";
               if (inputId.IsEmpty())
               {
                   throw new DAL.Exceptions.EmpNotFound("Employee Id can't be null");
               }
                DAL.Models.Employee selectedEmp =await _employee.GetEmployeeById(inputId);
                BAL.DTO.Employee emp =await TakeInput("Edit");
               await _employee.UpdateEmployee(emp,selectedEmp);
               Printer.Print(true, "Employee Updated");
           }
            catch (DAL.Exceptions.EmpNotFound ex)
            {
                Printer.Print(true, ex.Message);
            }
            catch (Exception ex)
           {
               Printer.Print(true, ex.Message);
           }
           

        }
        public async Task DisplayEmployeeList()
        {
           try
           {
               List<DAL.Models.Employee> employeeList =await _employee.GetEmployees();
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
           catch (Exception ex)
           {
               Printer.Print(true, ex.Message);
           }

        }

        private void DisplayEmployee(DAL.Models.Employee emp)
        {
           Printer.Print(true, $"Emp Id: {emp.Id} -- Full Name: {emp.FirstName} {emp.LastName} -- Department: {emp.Department.Name} -- Role: {emp.Role.Name} -- Email: {emp.Email} -- Location: {emp.Location.Name} -- JoiningDate: {emp.JoiningDate} -- Manager: {emp.Manager?.FirstName} {emp.Manager?.LastName} -- Project: {emp.Project?.Name} -- DOB: {emp.Dob} -- Mobile: {emp.Mobile}");
        }

        private async Task DisplaySelectedEmp()
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
               DAL.Models.Employee emp =await _employee.GetEmployeeById(inputId);
               DisplayEmployee(emp);
           }
           catch (DAL.Exceptions.EmpNotFound ex)
           {
               Printer.Print(true, ex.Message);
           }
           catch (Exception ex)
           {
               Printer.Print(true, ex.Message);
           }
        }

        public async Task DeleteEmployee()
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
                    await _employee.DeleteEmployee(inputId);
                    Printer.Print(true, "Employee deleted");
                }
           }
           catch (DAL.Exceptions.EmpNotFound ex)
           {
               Printer.Print(true, ex.Message);
           }
           catch (Exception ex)
           {
               Printer.Print(true, ex.Message);
           }
        }
    }

}
