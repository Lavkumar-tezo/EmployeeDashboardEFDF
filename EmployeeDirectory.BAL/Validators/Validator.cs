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
    public class Validator(IRoleProvider role, IGenericProvider<Location> loc,
        IGenericProvider<Department> dept, IGenericProvider<Project> proj,IEmployeeProvider emp) :IValidator
    {
        private readonly IGenericProvider<Project> _proj = proj;
        private readonly IGenericProvider<Department> _dept = dept;
        private readonly IEmployeeProvider _emp = emp;
        private readonly IRoleProvider _role = role;
        private readonly IGenericProvider<Location> _loc = loc;

        private static bool ValidateEmail(string value, string key)
        {
           bool check = IsFieldEmpty(value, key);
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
           if (input.Length != 10 || input.All(char.IsDigit))
           {
               MessagesInputStore.validationMessages[key] = "Mobile number should of 10 characters and contains digit only";
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
               bool check = IsFieldEmpty(value, key);
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

        private bool ValidateInput(string key, string value, Dictionary<string, string> getStaticData,string mode)
        {
            value = value.Trim().ToLower();
            if (value.IsEmpty() && key.Equals("Project"))
            {
                MessagesInputStore.validationMessages.Remove(key);
                return true;
            }
            else if (value.IsEmpty())
            {
                MessagesInputStore.validationMessages[key]=$"{key} : Required fields can't be null";
                return false;
            }
            if ((MessagesInputStore.validationMessages.ContainsKey("Department") || MessagesInputStore.validationMessages.ContainsKey("Location")) && string.Equals(mode, "Add") && string.Equals(key,"Role"))
            {
                MessagesInputStore.validationMessages["Role"] = "Select valid department and location first for applying role";
                return false;
            }
            foreach (var item in getStaticData)
            {
                if (item.Value.ToLower().Equals(value.ToLower()))
                {
                    MessagesInputStore.inputFieldValues[key] = item.Key;
                    switch (key)
                    {
                        case "Department":
                            ModelKeyStore.deptId = item.Key;
                            ModelKeyStore.deptName = item.Value;
                            break;
                        case "Location":
                            ModelKeyStore.locId = item.Key;
                            ModelKeyStore.locName = item.Value;
                            break;
                        case "Role":
                            ModelKeyStore.roleId = item.Key;
                            ModelKeyStore.roleName = item.Value;
                            break;
                        case "Project":
                            ModelKeyStore.projectId = item.Key;
                            break;
                    }

                    MessagesInputStore.validationMessages.Remove(key);
                    return true;
                }
            }
            return false;
        }

        private bool ValidateEmployeeRole(string mode)
        {
            string dept = MessagesInputStore.inputFieldValues["Department"];
            string loc = MessagesInputStore.inputFieldValues["Location"];
            string role = MessagesInputStore.inputFieldValues["Role"];
            Dictionary<string, string> deptList = _dept.GetIdName();
            Dictionary<string, string> locList = _loc.GetIdName();
            Dictionary<string, string> roleList = _role.GetIdName();
            bool check = true;
            if (string.Equals(mode, "Edit"))
            {
                dept = (dept.IsEmpty()) ? ModelKeyStore.deptName : dept;
                loc = (loc.IsEmpty()) ? ModelKeyStore.locName : loc;
                role = (role.IsEmpty()) ? ModelKeyStore.roleName : role;
            }
            check=ValidateInput("Department", dept, deptList, mode) && check;
            check = ValidateInput("Location", loc, locList, mode) && check;
            check = ValidateInput("Role", role, roleList, mode) && check;
            if (!check)
            {
                return check;
            }
            Role selectedRole = _role.GetRole(ModelKeyStore.roleId);
            Department selectedDepartment=_dept.Get(ModelKeyStore.deptId);
            Location selectedLocation=_loc.Get(ModelKeyStore.locId);
            if(selectedRole.Departments.Contains(selectedDepartment) && selectedRole.Locations.Contains(selectedLocation))
            {
                return true;
            }
            if (string.Equals(mode, "Add"))
            {
                dept = string.Join(", ", deptList.Values);
                loc = string.Join(", ",locList.Values);
                role = string.Join(", ",roleList.Values);
                MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departaments : {dept}";
                MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Available Locations : {loc}";
                MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Roles : {role}";
            }
            else
            {
                if(!dept.IsEmpty() && !loc.IsEmpty())
                {
                    List<Department> depts = selectedRole.Departments.ToList();
                    dept = string.Join(", ", depts.Select(d => d.Name));
                    List<Location> locs = selectedRole.Locations.ToList();
                    loc = string.Join(", ", locs.Select(d => d.Name));
                    MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departaments : {dept}";
                    MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Availabl Locations : {loc}";
                }
                else if(!dept.IsEmpty() && !role.IsEmpty())
                {
                    List<Department> depts = selectedRole.Departments.ToList();
                    dept = string.Join(", ", depts.Select(d => d.Name));
                    List<Role> roles=selectedLocation.Roles.ToList();
                    role = string.Join(", ", roles.Select(d => d.Name));
                    MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departaments : {dept}";
                    MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Role : {role}";
                }
                else if(!loc.IsEmpty() && !role.IsEmpty())
                {
                    List<Location> locs = selectedRole.Locations.ToList();
                    loc = string.Join(", ", locs.Select(d => d.Name));
                    List<Role> roles = selectedDepartment.Roles.ToList();
                    role = string.Join(", ", roles.Select(d => d.Name));
                    MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Availabl Locations : {loc}";
                    MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Role : {role}";
                }
                else if (!dept.IsEmpty())
                {
                    List<Department> depts = selectedRole.Departments.ToList();
                    dept = string.Join(", ", depts.Select(d => d.Name));
                    MessagesInputStore.validationMessages["Department"] = $"Selected Combination not found, Available Departaments : {dept}";
                }
                else if (!loc.IsEmpty())
                {
                    List<Location> locs = selectedRole.Locations.ToList();
                    loc = string.Join(", ", locs.Select(d => d.Name));
                    MessagesInputStore.validationMessages["Location"] = $"Selected Combination not found, Available Locations : {loc}";
                }
                else if (!role.IsEmpty())
                {
                    List<Role> roles = selectedDepartment.Roles.ToList();
                    role = string.Join(", ", roles.Select(d => d.Name));
                    MessagesInputStore.validationMessages["Role"] = $"Selected Combination not found, Available Roles : {role}";
                }
            }
            return false;
           
        }

        public bool ValidateManager(string key, string value)
        {
            string pattern = @"^TZ\d{4}$";
            bool check=Regex.IsMatch(value, pattern);
            if (!check)
            {
                MessagesInputStore.validationMessages[key] = "Manager id should be in TZXXXX Format";
                return check;
            }
            List<Employee> list= _emp.GetManagers();
            foreach( Employee emp in list )
            {
                if(emp.Id.Equals(value))
                {
                    MessagesInputStore.validationMessages.Remove(key);
                    return true;
                }
            }
            MessagesInputStore.validationMessages[key] = "Selected ID not found";
            return false;
        }

        public bool ValidateEmployeeInputs(string mode, ref bool isAllInputCorrect)
        {
           bool isAllValid = true;
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
                       isAllValid = IsFieldEmpty(input.Value ?? "", input.Key) && isAllValid;
                   }
                   else if (input.Key.Equals("Email"))
                   {
                       isAllValid = ValidateEmail(input.Value ?? "", input.Key) && isAllValid;
                   }
                   else if (input.Key.Equals("JoinDate") || input.Key.Equals("DOB"))
                   {
                        /*                      isAllValid = ValidateDate(input.Value ?? "", input.Key) && isAllValid;*/
                        isAllValid = true && isAllValid;

                    }
                    else if ((input.Key.Equals("Role") || input.Key.Equals("Location") || input.Key.Equals("Department")))
                   {
                        isAllValid = ValidateEmployeeRole(mode) && isAllValid;
                   }
                   else if (input.Key.Equals("Project"))
                   {
                       isAllValid = ValidateInput(input.Key, input.Value ?? "", _proj.GetIdName(),mode) && isAllValid;
                   }
                   else if (input.Key.Equals("Mobile"))
                   {
                       isAllValid = ValidatePhone(input.Value ?? "", input.Key) && isAllValid;
                   }
                   else if (input.Key.Equals("Manager"))
                   {
                        if(input.Value.IsEmpty())
                        {
                            MessagesInputStore.validationMessages.Remove(input.Key);
                        }
                        else
                        {
                            isAllValid = ValidateManager(input.Key,input.Value) && isAllValid;
                        }
                        
                    }
               }
           }
           return isAllValid;
        }

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
                locList =_loc.GetIdName();
            }
            else
            {
                locList =_dept.GetIdName();
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
