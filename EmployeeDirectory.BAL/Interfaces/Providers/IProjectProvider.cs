using EmployeeDirectory.DAL.Models;


namespace EmployeeDirectory.BAL.Interfaces.Providers
{
    public interface IProjectProvider
    {
        public List<Project> GetList();

        public Dictionary<string, string> GetProjects();
    }
}
