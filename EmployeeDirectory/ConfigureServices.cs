using Microsoft.Extensions.DependencyInjection;
using EmployeeDirectory.BAL.Providers;
using EmployeeDirectory.BAL.Validators;
using EmployeeDirectory.DAL.Repositories;
using EmployeeDirectory.DAL.Models;
using EmployeeDirectory.Interfaces.Views;
using EmployeeDirectory.DAL.Interfaces;
using EmployeeDirectory.BAL.Interfaces.Providers;
using EmployeeDirectory.BAL.Interfaces.Validators;
using EmployeeDirectory.BAL.Interfaces.Helpers;
using EmployeeDirectory.Interfaces.Helpers;
using EmployeeDirectory.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
namespace EmployeeDirectory
{
    public class ConfigureServices
    {
        public static IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<IEmployeeView, Views.Employee>();
            services.AddScoped<IChoiceTaker,ChoiceTaker>();
            services.AddScoped<IRoleView, Views.Role>();
            services.AddScoped<IValidator, Validator>();
            services.AddScoped<IEmployeeValidator, EmployeeValidator>();
            services.AddScoped<IRoleValidator, RoleValidator>();
            services.AddScoped<IEmployeeProvider, EmployeeProvider>();
            services.AddScoped<IGetProperty, GetProperty>();
            services.AddScoped<IRoleProvider, RoleProvider>();
            services.AddScoped<IProvider<Department>, DepartmentProvider>();
            services.AddScoped<IProvider<Project>, ProjectProvider>();
            services.AddScoped<IProvider<Location>, LocationProvider>();
            services.AddScoped<IRepository<Role>, RoleRepository>();
            services.AddScoped<IRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IRepository<Department>, DepartmentRepository>();
            services.AddScoped<IRepository<Project>, ProjectRepository>();
            services.AddScoped<IRepository<Location>, LocationRepository>();
            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("app-settings.json").Build();
            string connectionString = configBuilder["connection:sql"]!;
            services.AddDbContext<LavDbEfdfContext>(Options =>
            {
                Options.UseSqlServer(connectionString);
            });
            services.AddScoped<MainMenu>();
            return services.BuildServiceProvider();
        }

    }
}
