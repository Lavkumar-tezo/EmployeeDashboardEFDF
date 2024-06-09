using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectory.DAL.Repositories
{
    public class ProjectRepository(LavDbEfdfContext context):IRepository<Project>
    {
        private readonly LavDbEfdfContext _dbEfContext = context;

        public async Task<List<Project>> GetAll()
        {
            List<Project> projects = await _dbEfContext.Projects.ToListAsync();
            return projects;
        }

        public async  Task<Project> Get(string id)
        {
            List<Project> projects = await GetAll();
            Project? project = projects.FirstOrDefault(proj => string.Equals(proj.Id, id));
            if (project != null)
            {
                return project;
            }
            throw new Exception("Selected Project Not found");
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Project project)
        {
            throw new NotImplementedException();
        }

        public async Task Add(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
