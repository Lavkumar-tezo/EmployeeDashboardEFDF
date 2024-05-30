using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.DAL.Repositories
{
    public class ProjectRepository(LavDbEfdfContext context):IRepository<Project>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public List<Project> GetAll()
        {
            return _dbEfContext.Projects.ToList();
        }

        public Project Get(string id)
        {
            return _dbEfContext.Projects.Find(Int32.Parse(id))!;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Project project)
        {
            throw new NotImplementedException();
        }

        public void Add(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
