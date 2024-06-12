using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.BAL.Interfaces.Validators;
using EmployeeDirectory.BAL.Providers;
using EmployeeDirectory.DAL.Models;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EmployeeDirectory.BAL.Validators
{
    public class EmployeeValidator(IRoleProvider role, IProvider<Location> loc,
        IProvider<Department> dept, IProvider<Project> proj, IEmployeeProvider emp, IValidator val) : IEmployeeValidator
    {
        private readonly IProvider<Project> _proj = proj;
        private readonly IProvider<Department> _dept = dept;
        private readonly IEmployeeProvider _emp = emp;
        private readonly IRoleProvider _role = role;
        private readonly IProvider<Location> _loc = loc;
        private readonly IValidator _val = val;

        private bool ValidateEmail(string value, string key)
        {
            if (_val.IsFieldEmpty(value, key))
            {
                return false;
            }
            try
            {
                MailAddress mail = new(value);
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            catch (FormatException ex)
            {
                MessagesInputStore.validationMessages[key] = $"Email : {ex.Message}";
                return false;
            }
        }

        private bool ValidatePhone(string input, string key)
        {
            if (input.IsEmpty())
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            if (input.Length != 10 || !input.All(char.IsDigit))
            {
                MessagesInputStore.validationMessages[key] = input.Length != 10 ? "Mobile number should be of 10 digits" : "Mobile number should contain digits only";
                return false;
            }
            MessagesInputStore.validationMessages.Remove(key);
            return true;
        }

        private static bool IsValidDateFormat(string value, string key)
        {
            if (DateTime.TryParseExact(value, "MM/dd/yyyy", null, DateTimeStyles.None, out DateTime result))
            {
                MessagesInputStore.inputFieldValues[key] = result.ToString("MM/dd/yyyy");
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            else
            {
                MessagesInputStore.validationMessages[key] = $"{key} : Date is not in mm/dd/yyyy format";
                return false;
            }
        }

        private bool ValidateDate(string value, string key)
        {
            if (key.Equals("DOB"))
            {
                if (value.IsEmpty())
                {
                    MessagesInputStore.validationMessages.Remove(key);
                    return true;
                }
                else
                {
                    return IsValidDateFormat(value, key);
                }
            }
            else
            {
                bool check = _val.IsFieldEmpty(value, key);
                if (check)
                {
                    return false;
                }
                else
                {
                    check = IsValidDateFormat(value, key);
                    return check;
                }
            }
        }

        private (bool, string) ValidateInput(string key, string value, Dictionary<string, string> getStaticData, string mode) // validating given value is present in the database or not
        {
            value = value.Trim().ToLower();
            if (value.IsEmpty() && key.Equals("Project"))
            {
                MessagesInputStore.validationMessages.Remove(key);
                return (true, "passed");
            }
            else if (value.IsEmpty())
            {
                MessagesInputStore.validationMessages[key] = $"{key} : Required fields can't be null";
                return (false, "Failed");
            }
            if ((MessagesInputStore.validationMessages.ContainsKey("Department") || MessagesInputStore.validationMessages.ContainsKey("Location")) && string.Equals(mode, "Add") && string.Equals(key, "Role"))
            {
                MessagesInputStore.validationMessages["Role"] = "Select valid department and location first for applying role";
                return (false, "Failed");
            }
            foreach (var item in getStaticData)
            {
                if (item.Value.ToLower().Equals(value.ToLower()))
                {
                    if (key.Equals("Project"))
                    {
                        MessagesInputStore.inputFieldValues[key] = item.Key;
                    }
                    MessagesInputStore.validationMessages.Remove(key);
                    return (true, item.Key);
                }
            }
            string message = "Selected " + key + " Not Found. Choose from these: ";
            message += string.Join(", ", getStaticData.Values);
            MessagesInputStore.validationMessages[key] = message;
            return (false, "Failed");
        }

        private async Task<bool> IsValidCombination(string mode, string dept, string loc, string role) // checking role,dept and location combination exists or not
        {
            bool check = true;
            if (string.Equals(mode, "Edit"))
            {
                dept = (dept.IsEmpty()) ? SelectedEmployee.deptName : dept;
                loc = (loc.IsEmpty()) ? SelectedEmployee.locName : loc;
                role = (role.IsEmpty()) ? SelectedEmployee.roleName : role;
            }
            bool isValid = true;
            Dictionary<string, string> deptList =await _dept.GetIdName();
            (isValid, string deptId) = ValidateInput("Department", dept, deptList, mode);
            check = isValid && check;
            Dictionary<string, string> locList =await _loc.GetIdName();
            (isValid, string locId) = ValidateInput("Location", loc, locList, mode);
            check = isValid && check;
            Dictionary<string, string> roleList =await _role.GetIdName();
            (isValid, string roleId) = ValidateInput("Role", role, roleList, mode);
            check = isValid && check;
            if (!check)
            {
                return check;
            }
            Role selectedRole =await _role.GetRole(roleId);
            Department selectedDepartment =await _dept.Get(deptId);
            Location selectedLocation =await _loc.Get(locId);
            bool isDeptContainRole = selectedDepartment.Roles.Any(role => role.Id == selectedRole.Id);
            bool isLocContainRole = selectedLocation.Roles.Any(role => role.Id == selectedRole.Id);
            if (isDeptContainRole && isLocContainRole)
            {
                MessagesInputStore.inputFieldValues["Role"] = selectedRole.Id;
                MessagesInputStore.inputFieldValues["Department"] = selectedDepartment.Id.ToString();
                MessagesInputStore.inputFieldValues["Location"] = selectedLocation.Id.ToString();
                return true;
            }
            if (string.Equals(mode, "Add"))
            {
                MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departments : {string.Join(", ", deptList.Values)}";
                MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Available Locations : {string.Join(", ", locList.Values)}";
                MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Roles : {string.Join(", ", roleList.Values)}";
            }
            else
            {
                if (!MessagesInputStore.inputFieldValues["Department"].IsEmpty() && !MessagesInputStore.inputFieldValues["Location"].IsEmpty()) // user has entered new department and location only
                {
                    List<Department> depts = selectedRole.Departments.ToList();
                    List<Location> locs = selectedRole.Locations.ToList();
                    MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departments : {string.Join(", ", depts.Select(d => d.Name))}";
                    MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Available Locations : {string.Join(", ", locs.Select(d => d.Name))}";
                }
                else if (!MessagesInputStore.inputFieldValues["Department"].IsEmpty() && !MessagesInputStore.inputFieldValues["Role"].IsEmpty()) // user has entered new department and Role only
                {
                    List<Department> depts = selectedRole.Departments.ToList();
                    List<Role> roles = selectedLocation.Roles.ToList();
                    MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departments : {string.Join(", ", depts.Select(d => d.Name))}";
                    MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Role : {string.Join(", ", roles.Select(d => d.Name))}";
                }
                else if (!MessagesInputStore.inputFieldValues["Location"].IsEmpty() && !MessagesInputStore.inputFieldValues["Role"].IsEmpty()) // user has entered new Role and location only
                {
                    List<Location> locs = selectedRole.Locations.ToList();
                    List<Role> roles = selectedDepartment.Roles.ToList();
                    MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Available Locations : {string.Join(", ", locs.Select(d => d.Name))}";
                    MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Role : {string.Join(", ", roles.Select(d => d.Name))}";
                }
                else if (!MessagesInputStore.inputFieldValues["Department"].IsEmpty()) // user has entered new department only
                {
                    List<Department> depts = selectedRole.Departments.ToList();
                    MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departments : {string.Join(", ", depts.Select(d => d.Name))}";
                }
                else if (!MessagesInputStore.inputFieldValues["Location"].IsEmpty()) // user has entered new location only
                {
                    List<Location> locs = selectedRole.Locations.ToList();
                    MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Available Locations : {string.Join(", ", locs.Select(d => d.Name))}";
                }
                else if (!MessagesInputStore.inputFieldValues["Role"].IsEmpty()) // user has entered new role only
                {
                    List<Role> roles = selectedDepartment.Roles.ToList();
                    MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Roles : {string.Join(", ", roles.Select(d => d.Name))}";
                }
            }
            return false;

        }

        public async Task<bool> ValidateManager(string key, string value)
        {
            value = value.ToUpper();
            if (!Regex.IsMatch(value, @"^TZ\d{4}$"))
            {
                MessagesInputStore.validationMessages[key] = "Manager id should be in TZXXXX Format";
                return false;
            }
            List<Employee> managers = await _emp.GetManagers();
            bool managerExists =managers.Any(emp => emp.Id.Equals(value));
            if (managerExists)
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }

            MessagesInputStore.validationMessages[key] = "Selected ID not found";
            return false;
        }

        public async Task<bool> ValidateEmployeeInputs(string mode, bool isAllInputCorrect)
        {
            bool isAllValid = true;
            bool isCombinationTrue = true;
            foreach (var input in MessagesInputStore.inputFieldValues)
            {
                if (!mode.Equals("Add") && input.Value!.IsEmpty())
                {
                    MessagesInputStore.validationMessages.Remove(input.Key);
                }
                else if (MessagesInputStore.validationMessages.ContainsKey(input.Key) || isAllInputCorrect)
                {
                    if (input.Key.Equals("FirstName") || input.Key.Equals("LastName"))
                    {
                        isAllValid = !_val.IsFieldEmpty(input.Value ?? "", input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("Email"))
                    {
                        isAllValid = ValidateEmail(input.Value ?? "", input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("JoinDate") || input.Key.Equals("DOB"))
                    {
                        isAllValid = ValidateDate(input.Value ?? "", input.Key) && isAllValid;

                    }
                    else if ((input.Key.Equals("Role") || input.Key.Equals("Location") || input.Key.Equals("Department")) && isCombinationTrue)
                    {
                        isCombinationTrue = !isCombinationTrue;
                        isAllValid =await IsValidCombination(mode, MessagesInputStore.inputFieldValues["Department"], MessagesInputStore.inputFieldValues["Location"], MessagesInputStore.inputFieldValues["Role"]) && isAllValid;
                    }
                    else if (input.Key.Equals("Project"))
                    {
                        (bool check, string message) = ValidateInput(input.Key, input.Value ?? "", await _proj.GetIdName(), mode);
                        isAllValid = isAllValid && check;
                        if (check && !string.Equals(message, "passed"))
                        {
                            isAllValid = isAllValid && check;
                        }
                    }
                    else if (input.Key.Equals("Mobile"))
                    {
                        isAllValid = ValidatePhone(input.Value ?? "", input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("Manager"))
                    {
                        if ((input.Value ?? "").IsEmpty())
                        {
                            MessagesInputStore.validationMessages.Remove(input.Key);
                        }
                        else
                        {
                            isAllValid =await ValidateManager(input.Key, input.Value!) && isAllValid;
                        }
                    }
                }
            }
            return isAllValid;
        }
    }
}
