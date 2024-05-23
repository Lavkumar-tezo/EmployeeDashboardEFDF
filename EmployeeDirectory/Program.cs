using Microsoft.Extensions.DependencyInjection;
namespace EmployeeDirectory
{
    internal class Program
    {
        public static void Main()
        {
            IServiceProvider serviceProvider = ConfigureServices.BuildServices();
            MainMenu menu= serviceProvider.GetRequiredService<MainMenu>();
            menu.ShowMainMenu();
        }

    }
}