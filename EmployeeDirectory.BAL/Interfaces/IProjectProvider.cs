using EmployeeDirectory.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectory.BAL.Interfaces
{
    public interface IProjectProvider
    {
        public List<Project> GetList();

        public Dictionary<string, string> GetProjects();
    }
}
