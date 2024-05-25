using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Providers
{
    public class ProjectProvider(IGenericRepository<Project> proj):IProjectProvider
    {
        private readonly IGenericRepository<Project> _proj = proj;

        public List<Project> GetList()
        {
            return _proj.GetAll();
        }

        public Dictionary<string, string> GetProjects()
        {
            List<Project> projects = GetList();
            Dictionary<string, string> projList = new Dictionary<string, string>();
            foreach (Project d in projects)
            {
                projList.Add(d.Id.ToString(), d.Name);
            }
            return projList;
        }
    }
}
