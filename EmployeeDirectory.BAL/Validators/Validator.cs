using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Globalization;
using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.BAL.Helper;
using EmployeeDirectory.BAL.Extension;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.BAL.Providers;
namespace EmployeeDirectory.BAL.Validators
{
    public class Validator(IRoleProvider role,IDepartmentProvider dept, IProjectProvider proj) : IValidator
    {
        private readonly IProjectProvider _proj=proj;
        private readonly IDepartmentProvider _dept = dept;
        private readonly IRoleProvider _role = role;

        private static bool ValidateEmail(string value, string key)
        {
            bool check = ValidateEmptyField(value, key);
            if (check)
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

        private static bool ValidatePhone(string input, string key)
        {
            if (input.IsEmpty())
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            if (input.Length != 10)
            {
                MessagesInputStore.validationMessages[key] = "Mobile number should of 10 digit";
                return false;
            }
            bool isDigit = input.All(char.IsDigit);
            if (!isDigit)
            {
                MessagesInputStore.validationMessages[key] = "Mobile number should contains digit only";
                return false;
            }
            else
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
        }



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


        private static bool ValidateEmptyField(string value, string key)
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


        private static bool IsValidDateFormat(string value, string key)
        {
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime result))
            {
                MessagesInputStore.inputFieldValues[key] = result.ToString();
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            else
            {
                MessagesInputStore.validationMessages[key] = $"{key} : Date is not in dd/mm/yyyy format";
                return false;
            }
        }

        private static bool ValidateDate(string value, string key)
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
                bool check = ValidateEmptyField(value, key);
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

        private (bool, string) ValidateRoleName(string value)
        {
            if (!IsAlphabeticSpace(value))
            {
                return (false, "Role name Should contains Alphabets only");
            }
            value = value.Trim().ToLower();
            if(MessagesInputStore.validationMessages.ContainsKey("Department"))
            {
                return (false, "select valid department first for adding role");
            }
            List<Role> roles = _role.GetRolesByDept(MessagesInputStore.inputFieldValues["Department"]);
            roles = (from role in roles where role.Name.ToLower().Equals(value) select role).ToList();
            if (roles.Count > 0)
            {
                return (false, "This role already exists in selected department");
            }
            return (true, "Role available");

        }

        private bool ValidateInput(string key, string value, Dictionary<string,string> getStaticData)
        {
            value = value.Trim().ToLower();
            if (value.IsEmpty() && key.Equals("Project"))
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            if (Int32.TryParse(value,out int check))
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
                if (key.Equals("Department"))
                {
                    MessagesInputStore.validationMessages["Role"] = "Select valid department first";
                }
                return false;

            }
            catch (FormatException ex)
            {
                MessagesInputStore.validationMessages[key] = $"{key} : {ex.Message}";
                return false;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public bool ValidateEmployeeRole(string value,string key,bool isEdit)
        {         
            string deptId;
            Dictionary<string,string> departments = _dept.GetDepartments();
            if (!isEdit)
            {
                bool check = ValidateInput("Department", MessagesInputStore.inputFieldValues["Department"], departments);
                if (!check)
                {
                    MessagesInputStore.validationMessages[key] = "Select a valid department First for role";
                    return false;
                }
                deptId = MessagesInputStore.inputFieldValues["Department"];
            }
            else
            {
                deptId= ModelKeyStore.deptId;                
            }
            List<Role> selectedRoles = _role.GetRolesByDept(deptId);
            string roleNames = "";
            foreach (Role role in selectedRoles)
            {
                if (role.Name.ToLower().Equals(value.ToLower()))
                {
                    MessagesInputStore.inputFieldValues["Role"] = role.Id;
                    MessagesInputStore.validationMessages.Remove("Role");
                    return true;
                }
                roleNames += $", {role.Name}";
            }
            string message = $"Selected {key} Not Found in selected department. Choose from these: {roleNames}";
            MessagesInputStore.validationMessages[key] = message;
            return false;
        }

        public bool ValidateDepartment(string key,string value)
        {
            Dictionary<string, string> deptlist = _dept.GetDepartments();
            if(Int32.TryParse(value,out int num) && num >0 && num<=deptlist.Count)
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            string deptValue = MessagesInputStore.inputFieldValues["Department"];
            string roleNames = "";
            foreach (KeyValuePair<string, string> item in deptlist)
            {
                if (item.Value.ToLower().Equals(value.ToLower()))
                {
                    MessagesInputStore.inputFieldValues[key] = item.Key;
                    MessagesInputStore.validationMessages.Remove(key);
                    return true;
                }
            }
            string message = $"Selected {key} Not Found in selected department. Choose from these: {roleNames}";
            message += string.Join(", ", deptlist.Values);
            MessagesInputStore.validationMessages[key] = message;
            return false;
        }

        public bool ValidateRole()
        {
            return true;
        }

        public bool ValidateEmployeeInputs(string mode, ref bool isAllInputCorrect)
        {
            bool isAllValid = true;
            foreach (var input in MessagesInputStore.inputFieldValues)
            {
                if(!mode.Equals("Add") && input.Key.Equals("Department") && !input.Value!.IsEmpty())
                {
                    isAllValid =ValidateDepartment(input.Key,input.Value!) && isAllValid;
                }
                else if (!mode.Equals("Add") && input.Key.Equals("Role"))
                {
                    if (MessagesInputStore.inputFieldValues["Department"].IsEmpty() && input.Value!.IsEmpty())
                    {
                        MessagesInputStore.validationMessages.Remove(input.Key);
                    }
                    else if (!MessagesInputStore.inputFieldValues["Department"].IsEmpty())
                    {
                        isAllValid = ValidateEmployeeRole(input.Value ?? "", input.Key, false) && isAllValid;
                    }
                    else
                    {
                        isAllValid = ValidateEmployeeRole(input.Value ?? "", input.Key, true) && isAllValid;
                    }
                }
                else if (!mode.Equals("Add") && input.Value!.IsEmpty())
                {
                    MessagesInputStore.validationMessages.Remove(input.Key);
                }
                else if (MessagesInputStore.validationMessages.ContainsKey(input.Key) || isAllInputCorrect)
                {
                    if (input.Key.Equals("FirstName") || input.Key.Equals("LastName") || input.Key.Equals("Location"))
                    {
                        isAllValid = ValidateEmptyField(input.Value??"", input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("Email"))
                    {
                        isAllValid = ValidateEmail(input.Value??"", input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("JoinDate") || input.Key.Equals("DOB"))
                    {
                        isAllValid = ValidateDate(input.Value??"", input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("Role"))
                    {
                        isAllValid = ValidateEmployeeRole(input.Value??"", input.Key,false) && isAllValid;
                    }
                    else if (input.Key.Equals("Project"))
                    {
                        isAllValid = ValidateInput(input.Key, input.Value??"",_proj.GetProjects()) && isAllValid;
                    }
                    else if (input.Key.Equals("Mobile"))
                    {
                        isAllValid = ValidatePhone(input.Value??"", input.Key) && isAllValid;
                    }
                }
            }
            return isAllValid;
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
                        isAllValid = ValidateEmptyField(input.Value, input.Key) && isAllValid;
                        try
                        {
                            if (!isAllValid)
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
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                    else if (input.Key.Equals("Location"))
                    {
                        isAllValid = ValidateEmptyField(input.Value, input.Key) && isAllValid;
                    }
                    else if (input.Key.Equals("Department"))
                    {
                        isAllValid = ValidateInput(input.Key, input.Value, _dept.GetDepartments()) && isAllValid;
                    }
                }
            }
            return isAllValid;
        }

    }
}
