using Microsoft.Extensions.DependencyInjection;
namespace EmployeeDirectory
{
    internal class Program
    {
        public static async Task Main()
        {
            IServiceProvider serviceProvider = ConfigureServices.BuildServices();
            MainMenu menu= serviceProvider.GetRequiredService<MainMenu>();
            await menu.ShowMainMenu();
        }

    }
}