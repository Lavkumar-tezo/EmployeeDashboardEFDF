using EmployeeDirectory.Helpers;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Interfaces.Views;
using EmployeeDirectory.BAL.Interfaces;
namespace EmployeeDirectory.Views
{
    public class Role(IValidator validator, IRoleProvider role, IGetProperty prop,IDepartmentProvider dept) : IRoleView
    {
        private readonly IValidator _validator = validator;
        private readonly IRoleProvider _roleProvider = role;
        private readonly IGetProperty _getProperty = prop;
        private readonly IDepartmentProvider _dept=dept ;

        public void ShowRoleMenu()
        {
            int input;

        startRoleMenu: Printer.Print(true, "---------Role Management Menu---------", "1. Add Role", "2. Display All", "3. Go Back");
            input = ChoiceTaker.CheckChoice(1, 3);
            switch (input)
            {
                case 1:
                    AddRole();
                    goto NextProcess;
                case 2:
                    DisplayRoleList();
                    goto NextProcess;
                case 3:
                    Printer.Print(true, "Welcome Back to Main Menu");
                    break;
            }
        NextProcess: Printer.Print(true, "Where do u want to go", "1. Go to Main Menu", "2. Go to Previous Menu");
            input = ChoiceTaker.CheckChoice(1, 2);
            switch (input)
            {
                case 1:
                    Printer.Print(true, "Welcome Back to Main Menu");
                    break;
                case 2:
                    goto startRoleMenu;

            }
        }

        public void AddRole()
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
                                Dictionary<string, string> list = _dept.GetDepartments();
                                string message = "Available Department : ";
                                message += string.Join(", ", list.Values);
                                message += "  -- Write full department name";
                                Printer.Print(true, message);
                            }
                            Printer.Print(false, $"{inputName} : ");
                            MessagesInputStore.inputFieldValues[inputName] = Console.ReadLine() ?? "";
                        }
                    }
                    isAllInputCorrect = _validator.ValidateRoleInputs(ref isAllInputCorrect);
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
                Printer.Print(true, ex.InnerException?.Message??"");
            }

        }

        public void DisplayRoleList()
        {
            try
            {
                List<DAL.Models.Role> RoleList = _roleProvider.GetRoles();
                if (RoleList.Count==0)
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

        public void DisplayRole(DAL.Models.Role role)
        {
            Console.WriteLine($"Name: {role.Name} -- Department: {role.Department.Name} -- Location: {role.Location} -- Description: {role.Description}");
        }
    }
}
