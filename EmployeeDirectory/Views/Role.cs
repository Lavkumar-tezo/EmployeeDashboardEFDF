using EmployeeDirectory.Helpers;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.Interfaces.Views;
using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.BAL.Interfaces.Validators;
using EmployeeDirectory.BAL.Interfaces.Helpers;
using EmployeeDirectory.Interfaces.Helpers;
namespace EmployeeDirectory.Views
{
    public class Role(IRoleValidator roleValidator, IRoleProvider role, IGetProperty prop, IProvider<Location> loc, IProvider<Department> dept, IChoiceTaker taker) :IRoleView
    {
        private readonly IChoiceTaker _taker = taker;
        private readonly IRoleValidator _roleValidator=roleValidator;
        private readonly IRoleProvider _roleProvider = role;
        private readonly IGetProperty _getProperty = prop;
        private readonly IProvider<Department> _dept =dept ;
        private readonly IProvider<Location> _loc =loc ;

        public void ShowRoleMenu()
        {
            int input;

        Printer.Print(true, "---------Role Management Menu---------", "1. Add Role", "2. Display All", "3. Go Back");
            input = _taker.CheckChoice(1, 3);
            switch (input)
            {
                case 1:
                    AddRole();
                    break;
                case 2:
                    DisplayRoleList();
                    break;
                case 3:
                    Printer.Print(true, "Welcome Back to Main Menu");
                    return;
            }
        Printer.Print(true, "Where do u want to go", "1. Go to Main Menu", "2. Go to Previous Menu");
            input = _taker.CheckChoice(1, 2);
            switch (input)
            {
                case 1:
                    Printer.Print(true, "Welcome Back to Main Menu");
                    return;
                case 2:
                    ShowRoleMenu();
                    break;
            }
        }

        private void AddRole()
        {
            try
            {
                List<string> inputFields = _getProperty.GetProperties("Role");
                bool isAllInputCorrect = true;

                do
                {
                    foreach (var inputName in inputFields)
                    {
                        if (MessagesInputStore.validationMessages.ContainsKey(inputName) || isAllInputCorrect)
                        {
                            if (inputName.Equals("Department"))
                            {
                                Dictionary<string, string> list = _dept.GetIdName();
                                string message = "Available Department : ";
                                message += string.Join(", ", list.Values);
                                message += "  -- Write full department name";
                                Printer.Print(true, message);
                            }
                            else if (inputName.Equals("Location"))
                            {
                                Dictionary<string, string> list = _loc.GetIdName();
                                string message = "Available Location : ";
                                message += string.Join(", ", list.Values);
                                message += "  -- Write Location names separated By Comma";
                                Printer.Print(true, message);
                            }
                            Printer.Print(false, $"{inputName} : ");
                            MessagesInputStore.inputFieldValues[inputName] = Console.ReadLine() ?? "";
                            
                        }
                    }
                    isAllInputCorrect =_roleValidator.ValidateRoleInputs(ref isAllInputCorrect);
                    if (!isAllInputCorrect)
                    {
                        foreach (var item in MessagesInputStore.validationMessages)
                        {
                            Printer.Print(true, item.Value);
                        }
                    }
                } while (!isAllInputCorrect);
                MessagesInputStore.validationMessages.Clear();
                _roleProvider.AddRole(MessagesInputStore.inputFieldValues);
                MessagesInputStore.inputFieldValues.Clear();
                Printer.Print(true, "Role Added");
            }
            catch (Exception ex)
            {
                Printer.Print(true, ex.InnerException?.Message ?? "not added");
            }

        }

        private void DisplayRoleList()
        {
            try
            {
                List<DAL.Models.Role> RoleList = _roleProvider.GetRoles();
                if (RoleList.Count == 0)
                {
                    Console.WriteLine("No role found");
                }
                else
                {
                    for (int i = 0; i < RoleList.Count; i++)
                    {
                        DisplayRole(RoleList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Printer.Print(true, ex.Message);
            }

        }

        private void DisplayRole(DAL.Models.Role role)
        {
            List<string> locs=role.Locations.Select(x => x.Name).ToList();
            string location = string.Join(",", locs);
            List<string> depts = role.Departments.Select(x => x.Name).ToList();
            string department = string.Join(",", depts);
            Console.WriteLine($"Name: {role.Name} -- Departments: {department} -- Locations: {location} -- Description: {role.Description}");
        }
    }
}
