using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Globalization;
using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.BAL.Providers;
using EmployeeDirectory.BAL.Interfaces.Providers;
namespace EmployeeDirectory.BAL.Validators
{
    public class Validator(IRoleProvider role,IDepartmentProvider dept, IProjectProvider proj,ILocationProvider loc):IValidator
    {
        private readonly IProjectProvider _proj = proj;
        private readonly IDepartmentProvider _dept = dept;
        private readonly IRoleProvider _role = role;
        private readonly ILocationProvider _loc = loc;

        //private static bool ValidateEmail(string value, string key)
        //{
        //    bool check = IsFieldEmpty(value, key);
        //    if (check)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        MailAddress mail = new(value);
        //        MessagesInputStore.validationMessages.Remove(key);
        //        return true;
        //    }
        //    catch (FormatException ex)
        //    {
        //        MessagesInputStore.validationMessages[key] = $"Email : {ex.Message}";
        //        return false;
        //    }
        //}

        //private static bool ValidatePhone(string input, string key)
        //{
        //    if (input.IsEmpty())
        //    {
        //        MessagesInputStore.validationMessages.Remove(key);
        //        return true;
        //    }
        //    if (input.Length != 10)
        //    {
        //        MessagesInputStore.validationMessages[key] = "Mobile number should of 10 digit";
        //        return false;
        //    }
        //    bool isDigit = input.All(char.IsDigit);
        //    if (!isDigit)
        //    {
        //        MessagesInputStore.validationMessages[key] = "Mobile number should contains digit only";
        //        return false;
        //    }
        //    else
        //    {
        //        MessagesInputStore.validationMessages.Remove(key);
        //        return true;
        //    }
        //}



        private static bool IsAlphabeticSpace(string input)
        {
            Regex regex = new Regex("^[a-zA-Z ]+$");
            return regex.IsMatch(input);
        }


        public static int ValidateOption(string value)
        {
            int option = Convert.ToInt32(value);
            return option;
        }


        private static bool IsFieldEmpty(string value, string key)
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


        //private static bool IsValidDateFormat(string value, string key)
        //{
        //    if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime result))
        //    {
        //        MessagesInputStore.inputFieldValues[key] = result.ToString();
        //        MessagesInputStore.validationMessages.Remove(key);
        //        return true;
        //    }
        //    else
        //    {
        //        MessagesInputStore.validationMessages[key] = $"{key} : Date is not in dd/mm/yyyy format";
        //        return false;
        //    }
        //}

        //private static bool ValidateDate(string value, string key)
        //{
        //    if (key.Equals("DOB"))
        //    {
        //        if (value.IsEmpty())
        //        {
        //            MessagesInputStore.validationMessages.Remove(key);
        //            return true;
        //        }
        //        else
        //        {
        //            return IsValidDateFormat(value, key);
        //        }
        //    }
        //    else
        //    {
        //        bool check = IsFieldEmpty(value, key);
        //        if (check)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            check = IsValidDateFormat(value, key);
        //            return check;
        //        }
        //    }
        //}

        private (bool, string) ValidateRoleName(string value)
        {
            ValidateRoleFields("Department", MessagesInputStore.inputFieldValues["Department"]);
            if (MessagesInputStore.validationMessages.ContainsKey("Department"))
            {
                return (false, "Role : select valid department first for adding role");
            }
            value = value.Trim().ToLower();
            if (value.IsEmpty())
            {
                return (false, "Role : Required fields can't be null");
            }
            if (!IsAlphabeticSpace(value))
            {
                return (false, "Role : Role name Should contains Alphabets only");
            }
            List<Role> roles = _role.GetRoles();
            roles = (from role in roles where role.Name.ToLower().Equals(value) select role).ToList();
            if (roles.Count > 0)
            {
                return (false, "Role : This role already exists in selected department");
            }
            return (true, "Role available");

        }

        private bool ValidateInput(string key, string value, Dictionary<string, string> getStaticData)
        {
            value = value.Trim().ToLower();
            if (value.IsEmpty() && key.Equals("Project"))
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            if (value.IsEmpty() && key.Equals("Department"))
            {
                MessagesInputStore.validationMessages["Department"]="Department : Required fields can't be null";
                return false;
            }
            if (Int32.TryParse(value, out int check))
            {
                return true;
            }
            try
            {
                foreach (var item in getStaticData)
                {
                    if (item.Value.ToLower().Equals(value.ToLower()))
                    {
                        MessagesInputStore.inputFieldValues[key] = item.Key;
                        MessagesInputStore.validationMessages.Remove(key);
                        return true;
                    }
                }
                string message = "Selected " + key + " Not Found. Choose from these: ";
                message += string.Join(", ", getStaticData.Values);
                MessagesInputStore.validationMessages[key] = message;
                return false;

            }
            catch (FormatException ex)
            {
                MessagesInputStore.validationMessages[key] = $"{key} : {ex.Message}";
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public bool ValidateEmployeeRole(string value, string key, bool isEdit)
        //{
        //    string deptId;
        //    Dictionary<string, string> departments = _dept.GetDepartments();
        //    if (!isEdit)
        //    {
        //        bool check = ValidateInput("Department", MessagesInputStore.inputFieldValues["Department"], departments);
        //        if (!check)
        //        {
        //            MessagesInputStore.validationMessages[key] = "Select a valid department First for role";
        //            return false;
        //        }
        //        deptId = MessagesInputStore.inputFieldValues["Department"];
        //    }
        //    else
        //    {
        //        deptId = ModelKeyStore.deptId;
        //    }
        //    List<Role> selectedRoles = _role.GetRolesByDept(deptId);
        //    string roleNames = "";
        //    foreach (Role role in selectedRoles)
        //    {
        //        if (role.Name.ToLower().Equals(value.ToLower()))
        //        {
        //            MessagesInputStore.inputFieldValues["Role"] = role.Id;
        //            MessagesInputStore.validationMessages.Remove("Role");
        //            return true;
        //        }
        //        roleNames += $", {role.Name}";
        //    }
        //    string message = $"Selected {key} Not Found in selected department. Choose from these: {roleNames}";
        //    MessagesInputStore.validationMessages[key] = message;
        //    return false;
        //}

        //public bool ValidateDepartment(string key, string value)
        //{
        //    Dictionary<string, string> deptlist = _dept.GetDepartments();
        //    if (Int32.TryParse(value, out int num) && num > 0 && num <= deptlist.Count)
        //    {
        //        MessagesInputStore.validationMessages.Remove(key);
        //        return true;
        //    }
        //    string deptValue = MessagesInputStore.inputFieldValues["Department"];
        //    string roleNames = "";
        //    foreach (KeyValuePair<string, string> item in deptlist)
        //    {
        //        if (item.Value.ToLower().Equals(value.ToLower()))
        //        {
        //            MessagesInputStore.inputFieldValues[key] = item.Key;
        //            MessagesInputStore.validationMessages.Remove(key);
        //            return true;
        //        }
        //    }
        //    string message = $"Selected {key} Not Found in selected department. Choose from these: {roleNames}";
        //    message += string.Join(", ", deptlist.Values);
        //    MessagesInputStore.validationMessages[key] = message;
        //    return false;
        //}

        //public bool ValidateEmployeeInputs(string mode, ref bool isAllInputCorrect)
        //{
        //    bool isAllValid = true;
        //    foreach (var input in MessagesInputStore.inputFieldValues)
        //    {
        //        if (!mode.Equals("Add") && input.Key.Equals("Department") && !input.Value!.IsEmpty())
        //        {
        //            isAllValid = ValidateDepartment(input.Key, input.Value!) && isAllValid;
        //        }
        //        else if (!mode.Equals("Add") && input.Key.Equals("Role"))
        //        {
        //            if (MessagesInputStore.inputFieldValues["Department"].IsEmpty() && input.Value!.IsEmpty())
        //            {
        //                MessagesInputStore.validationMessages.Remove(input.Key);
        //            }
        //            else if (!MessagesInputStore.inputFieldValues["Department"].IsEmpty())
        //            {
        //                isAllValid = ValidateEmployeeRole(input.Value ?? "", input.Key, false) && isAllValid;
        //            }
        //            else
        //            {
        //                isAllValid = ValidateEmployeeRole(input.Value ?? "", input.Key, true) && isAllValid;
        //            }
        //        }
        //        else if (!mode.Equals("Add") && input.Value!.IsEmpty())
        //        {
        //            MessagesInputStore.validationMessages.Remove(input.Key);
        //        }
        //        else if (MessagesInputStore.validationMessages.ContainsKey(input.Key) || isAllInputCorrect)
        //        {
        //            if (input.Key.Equals("FirstName") || input.Key.Equals("LastName") || input.Key.Equals("Location"))
        //            {
        //                isAllValid = IsFieldEmpty(input.Value ?? "", input.Key) && isAllValid;
        //            }
        //            else if (input.Key.Equals("Email"))
        //            {
        //                isAllValid = ValidateEmail(input.Value ?? "", input.Key) && isAllValid;
        //            }
        //            else if (input.Key.Equals("JoinDate") || input.Key.Equals("DOB"))
        //            {
        //                isAllValid = ValidateDate(input.Value ?? "", input.Key) && isAllValid;
        //            }
        //            else if (input.Key.Equals("Role"))
        //            {
        //                isAllValid = ValidateEmployeeRole(input.Value ?? "", input.Key, false) && isAllValid;
        //            }
        //            else if (input.Key.Equals("Project"))
        //            {
        //                isAllValid = ValidateInput(input.Key, input.Value ?? "", _proj.GetProjects()) && isAllValid;
        //            }
        //            else if (input.Key.Equals("Mobile"))
        //            {
        //                isAllValid = ValidatePhone(input.Value ?? "", input.Key) && isAllValid;
        //            }
        //        }
        //    }
        //    return isAllValid;
        //}

        private bool ValidateRoleFields(string key, string value)
        {
            bool check = IsFieldEmpty(value, key);
            if (check)
            {
                return false;
            }
            string[] words = value.Split(',').Select(word => word.Trim().ToLower()).ToArray();
            Dictionary<string, string> locList = new Dictionary<string, string>();
            if (string.Equals(key,"Location"))
            {
                locList =_loc.GetLocations();
            }
            else
            {
                locList =_dept.GetDepartments();
            }
            List<string> locIds = new ();
            foreach(string word in words)
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

        public bool ValidateRoleInputs(ref bool isAllInputCorrect)
        {
            bool isAllValid = true;

            foreach (var input in MessagesInputStore.inputFieldValues)
            {
                if (MessagesInputStore.validationMessages.ContainsKey(input.Key) || isAllInputCorrect)
                {
                    if (input.Key.Equals("Name"))
                    {
                        (isAllValid, string message) = ValidateRoleName(input.Value);
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
                        isAllValid = ValidateRoleFields(input.Key, input.Value) && isAllValid;
                    }
                }
            }
            return isAllValid;
        }

    }
}
