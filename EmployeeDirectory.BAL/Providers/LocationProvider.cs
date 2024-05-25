using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Providers
{
    public class LocationProvider(IGenericRepository<Location> loc):ILocationProvider
    {
        private readonly IGenericRepository<Location> _loc = loc;

        public List<Location> GetList()
        {
            return _loc.GetAll();
        }
        public Dictionary<string, string> GetLocations()
        {
            List<Location> departments = GetList();
            Dictionary<string, string> deptList = new Dictionary<string, string>();
            foreach (Location d in departments)
            {
                deptList.Add(d.Id.ToString(), d.Name);
            }
            return deptList;
        }
    }
}
