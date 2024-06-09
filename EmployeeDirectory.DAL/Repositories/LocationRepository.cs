using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectory.DAL.Repositories
{
    public class LocationRepository(LavDbEfdfContext context):IRepository<Location>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public async Task<List<Location>> GetAll()
        {
            List<Location> locations = await _dbEfContext.Locations.ToListAsync();
            return locations;
        }

        public async Task<Location> Get(string id)
        {
            List<Location> locations = await GetAll();
            Location? location = locations.FirstOrDefault(loc => string.Equals(loc.Id, id));
            if (location != null)
            {
                return location;
            }
            throw new Exception("Selected Location Not found");
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Location location)
        {
            throw new NotImplementedException();
        }

        public async Task Add(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
