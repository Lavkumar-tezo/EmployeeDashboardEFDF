using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Providers
{
    public class LocationProvider(IRepository<Location> loc):IProvider<Location>
    {
        private readonly IRepository<Location> _loc = loc;

        public List<Location> GetList()
        {
            return _loc.GetAll();
        }

        public Location Get(string id)
        {
            int locId = Int32.Parse(id);
            List<Location> list = GetList();
            Location loc = list.First(x => x.Id == locId);
            return loc;
        }

        public Dictionary<string, string> GetIdName()
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
