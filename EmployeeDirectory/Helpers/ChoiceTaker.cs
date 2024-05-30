using EmployeeDirectory.BAL.Interfaces.Validators;
using EmployeeDirectory.Interfaces.Helpers;
namespace EmployeeDirectory.Helpers
{
    internal class ChoiceTaker(IValidator val):IChoiceTaker
    {
        private readonly IValidator _validator=val;

        /// <summary>
        /// Take input from user and check its range 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>integer value</returns>

        public int CheckChoice(int start, int end)
        {
            bool isInputValid;
            int input;

            do
            {
                Printer.Print(false, "Enter the choice : ");
                try
                {
                    input =_validator.ValidateOption(Console.ReadLine() ?? "");
                    if (input < start || input > end)
                    {
                        isInputValid = false;
                        Printer.Print(true, $"Please Enter Valid Input from {start} to {end}");
                    }
                    else
                    {
                        return input;
                    }
                }
                catch (FormatException ex)
                {
                    isInputValid = false;
                    Printer.Print(true, ex.Message);
                }
            } while (!isInputValid);
            return 0;
        }

    }
}