
using Microsoft.Extensions.DependencyInjection;

using FD.ConsoleAppCore.Contracts;
using FD.ConsoleAppCore.Services;
using FD.Library.Contracts;
using FD.Library.Services;

namespace FD.ConsoleAppCore
{
    internal class Program
    {
        private static void Main()
        {
            //setup DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IMenuServices, MenuServices>()
                .AddSingleton<IDepthChartServices, DepthChartServices>()
                .BuildServiceProvider();
           
            var menu = serviceProvider.GetService<IMenuServices>();
            var showMenu = true;
            while (showMenu)
            {
                if (menu != null) showMenu = menu.MainMenu();
            }


        }
    }
}
