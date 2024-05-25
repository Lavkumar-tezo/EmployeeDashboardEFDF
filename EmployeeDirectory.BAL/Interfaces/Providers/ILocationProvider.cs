using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface ILocationProvider
    {
        public List<Location> GetList();

        public Dictionary<string, string> GetLocations();
    }
}
