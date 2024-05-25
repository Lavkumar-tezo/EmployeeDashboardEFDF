using Microsoft.Extensions.DependencyInjection;
using EmployeeDirectory.BAL.Interfaces;
using EmployeeDirectory.BAL.Providers;
using EmployeeDirectory.BAL.Validators;
using EmployeeDirectory.DAL.Repositories;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.BAL.Interfaces.Views;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.BAL.Interfaces.Providers;
namespace EmployeeDirectory
{
    public class ConfigureServices
    {
        public static IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();

           // services.AddScoped<IEmployeeView, Views.Employee>();
            services.AddScoped<IRoleView, Views.Role>();
            services.AddScoped<IValidator, Validator>();
           // services.AddScoped<IEmpProvider, EmployeeProvider>();
            services.AddScoped<IGetProperty, GetProperty>();
            services.AddScoped<IRoleProvider, RoleProvider>();
            services.AddScoped<IDepartmentProvider, DepartmentProvider>();
            services.AddScoped<IProjectProvider, ProjectProvider>();
            services.AddScoped<ILocationProvider, LocationProvider>();
            services.AddScoped<IGenericRepository<Role>, RoleRepository>();
            //services.AddScoped<IGenericRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IGenericRepository<Department>, DepartmentRepository>();
            services.AddScoped<IGenericRepository<Project>, ProjectRepository>();
            services.AddScoped<IGenericRepository<Location>, LocationRepository>();
            services.AddDbContext<LavDbEfdfContext>();
            services.AddScoped<MainMenu>();
            return services.BuildServiceProvider();
        }

    }
}
