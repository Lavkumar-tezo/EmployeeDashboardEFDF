using System.Reflection;
using EmployeeDirectory.BAL.DTO;
using EmployeeDirectory.BAL.Interfaces.Helpers;
namespace EmployeeDirectory.BAL.Providers
{
    public class GetProperty:IGetProperty
    {
        public List<string> GetProperties(string className)
        {
            if (className.Equals("Employee"))
            {
                return typeof(Employee).GetProperties().Select(prop => prop.Name).ToList();
            }
            else
            {
                return typeof(Role).GetProperties().Select(prop => prop.Name).ToList();
            }
        }

        public Dictionary<string, string> GetValueFromObject<T>(T obj)
        {
            var objectKeyValues = new Dictionary<string, string>();

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (property.GetIndexParameters().Length == 0)
                {
                    string propertyName = property.Name;
                    if (propertyName.Equals("DepartmentId"))
                    {
                        propertyName = "Department";
                    }
                    else if (propertyName.Equals("RoleId"))
                    {
                        propertyName="Role";
                    }
                    else if (propertyName.Equals("ProjectId"))
                    {
                        propertyName = "Project";
                    }
                    else if (propertyName.Equals("LocationId"))
                    {
                        propertyName = "Location";
                    }
                    else if (propertyName.Equals("Role") || propertyName.Equals("Department") || propertyName.Equals("Project") || propertyName.Equals("Location"))
                    {
                        continue;
                    }
                    object propertyValue = property.GetValue(obj) ?? "default";
                    string value = propertyValue.ToString() ?? "";
                    objectKeyValues.Add(propertyName, value);
                }
            }
            return objectKeyValues;
        }
    }
}
