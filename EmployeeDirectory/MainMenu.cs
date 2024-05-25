using EmployeeDirectory.Helpers;
using EmployeeDirectory.BAL.Validators;
using EmployeeDirectory.BAL.Interfaces.Views;

namespace EmployeeDirectory
{
    public class MainMenu(IRoleView role)
    {
        //private readonly IEmployeeView emp = emp;
        private readonly IRoleView _role = role;

        public void ShowMainMenu()
        {
            int input;
            bool isValidInput;

            Printer.Print(true, "-----------------Welcome to Employee Directory-----------------");
            do
            {
                Printer.Print(true, "1. Employee Management", "2. Role Management", "3. Exit");
                Printer.Print(false, "Enter the choice (Numeric): ");
                try
                {
                    input = Validator.ValidateOption(Console.ReadLine() ?? "");

                    if (input < 1 || input > 3)
                    {
                        isValidInput = false;
                        Printer.Print(true, "Please enter valid input ranging from 1 to 3");
                    }
                    switch (input)
                    {
                        case 1:
                            isValidInput = false;
                           // emp.ShowEmployeeMenu();
                            break;
                        case 2:
                            isValidInput = false;
                            _role.ShowRoleMenu();
                            break;
                        default:
                            Printer.Print(true, "Program Ended");
                            isValidInput = true;
                            break;
                    }
                }
                catch (FormatException ex)
                {
                    isValidInput = false;
                    Printer.Print(true, ex.Message);
                }

            } while (!isValidInput);
        }
    }
}
