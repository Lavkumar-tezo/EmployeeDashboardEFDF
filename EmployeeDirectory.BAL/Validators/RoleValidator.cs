using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.BAL.Interfaces.Validators;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Validators
{
    public class RoleValidator(IRoleProvider role, IProvider<Location> loc,
        IProvider<Department> dept,IValidator val) :IRoleValidator
    {
        private readonly IProvider<Department> _dept = dept;
        private readonly IRoleProvider _role = role;
        private readonly IProvider<Location> _loc = loc;
        private readonly IValidator _val = val;

        private async Task<bool> ValidateRoleFields(string key, string value)
        {
            bool check = _val.IsFieldEmpty(value, key);
            if (check)
            {
                return false;
            }
            string[] words = value.Split(',').Select(word => word.Trim().ToLower()).ToArray();
            Dictionary<string, string> locList = new Dictionary<string, string>();
            if (string.Equals(key, "Location"))
            {
                locList =await _loc.GetIdName();
            }
            else
            {
                locList =await _dept.GetIdName();
            }
            List<string> locIds = new();
            foreach (string word in words)
            {
                var loc = locList.FirstOrDefault(x => string.Equals(x.Value.Trim(), word, StringComparison.OrdinalIgnoreCase));
                if (!loc.Equals(default(KeyValuePair<string, string>)))
                {
                    locIds.Add(loc.Key);
                }
                else
                {
                    MessagesInputStore.validationMessages[key] = $"{key} : select available {key} only";
                    return false;
                }
            }
            MessagesInputStore.validationMessages.Remove(key);
            MessagesInputStore.inputFieldValues[key] = string.Join(",", locIds);
            return true;
        }

        private async Task<(bool, string)> ValidateRoleName(string value)
        {
            await ValidateRoleFields("Department", MessagesInputStore.inputFieldValues["Department"]);
            if (MessagesInputStore.validationMessages.ContainsKey("Department"))
            {
                return (false, "Role : select valid department first for adding role");
            }
            value = value.Trim().ToLower();
            if (value.IsEmpty())
            {
                return (false, "Role : Required fields can't be null");
            }
            if (!_val.IsAlphabeticSpace(value))
            {
                return (false, "Role : Role name Should contains Alphabets only");
            }
            List<Role> roles =await _role.GetRoles();
            roles = (from role in roles where role.Name.ToLower().Equals(value) select role).ToList();
            if (roles.Count > 0)
            {
                return (false, "Role : This role already exists in selected department");
            }
            return (true, "Role available");
        }

        public async Task<bool> ValidateRoleInputs(bool isAllInputCorrect)
        {
            bool isAllValid = true;

            foreach (var input in MessagesInputStore.inputFieldValues)
            {
                if (MessagesInputStore.validationMessages.ContainsKey(input.Key) || isAllInputCorrect)
                {
                    if (input.Key.Equals("Name"))
                    {
                        (isAllValid, string message) =await ValidateRoleName(input.Value);
                        if (isAllValid)
                        {
                            MessagesInputStore.validationMessages.Remove(input.Key);
                        }
                        else
                        {
                            MessagesInputStore.validationMessages[input.Key] = message;
                        }
                    }
                    else if (input.Key.Equals("Location"))
                    {
                        isAllValid =await ValidateRoleFields(input.Key, input.Value) && isAllValid;
                    }
                }
            }
            return isAllValid;
        }

    }
}
