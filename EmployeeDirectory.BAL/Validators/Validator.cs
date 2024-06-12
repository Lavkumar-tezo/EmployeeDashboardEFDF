using System.Text.RegularExpressions;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.BAL.Interfaces.Validators;
namespace EmployeeDirectory.BAL.Validators
{
    public class Validator() :IValidator
    {

        public bool IsAlphabeticSpace(string input)
        {
            Regex regex = new Regex("^[a-zA-Z ]+$");
            return regex.IsMatch(input);
        }

        public int ValidateOption(string value)
        {
            int option = Convert.ToInt32(value);
            return option;
        }

        public bool IsFieldEmpty(string value, string key)
        {
            bool check = value.IsEmpty();
            if (check)
            {
                MessagesInputStore.validationMessages[key] = $"{key} : Required Field can't be null";
            }
            else
            {
                MessagesInputStore.validationMessages.Remove(key);
                
            }
            return check;
        }

    }
}
