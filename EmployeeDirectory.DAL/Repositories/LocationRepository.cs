using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.DAL.Repositories
{
    public class LocationRepository(LavDbEfdfContext context):IGenericRepository<Location>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public List<Location> GetAll()
        {
            return _dbEfContext.Locations.ToList();
        }

        public Location Get(string id)
        {
            return _dbEfContext.Locations.Find(Int32.Parse(id))!;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Location location)
        {
            throw new NotImplementedException();
        }

        public void Add(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
