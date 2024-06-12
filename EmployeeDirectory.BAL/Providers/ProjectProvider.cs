using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.DAL.Models;

namespace EmployeeDirectory.BAL.Providers
{
    public class ProjectProvider(IRepository<Project> proj):IProvider<Project>
    {
        private readonly IRepository<Project> _proj = proj;

        public async Task<List<Project>> GetList()
        {
            List<Project> list =await _proj.GetAll();
            return list;
        }

        public async Task<Project> Get(string id)
        {
            int prjectId=Int32.Parse(id);
            List<Project> list =await GetList();
            Project project = list.First(x=> x.Id==prjectId);
            return project;
        }

        public async Task<Dictionary<string, string>> GetIdName()
        {
            List<Project> projects =await GetList();
            Dictionary<string, string> projList = new Dictionary<string, string>();
            foreach (Project d in projects)
            {
                projList.Add(d.Id.ToString(), d.Name);
            }
            return projList;
        }
    }
}
