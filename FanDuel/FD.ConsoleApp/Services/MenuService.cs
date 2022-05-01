
using System.Runtime.CompilerServices;
using FD.Library.Contracts;
using FD.Library.Models;
using FD.Library.Services;

namespace FD.ConsoleApp.Services
{
    public class MenuService
    {
        private readonly IDepthChartServices _depthChartServices;

        private List<DepthChart> _depthChartList;

        public MenuService(IDepthChartServices depthChartServices)
        {
            _depthChartServices = depthChartServices;
            _depthChartList = new List<DepthChart>();
        }

        public bool MainMenu()
        {
            
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) AddPlayerToDepthChart");
            Console.WriteLine("2) GetBackups");
            Console.WriteLine("3) getFullDepthChart");
            Console.WriteLine("4) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    _depthChartList = AddPlayerToDepthChart();
                    return true;
                case "2":
                    
                    return true;
                case "3":
                    return true;
                case "4":
                    return true;
                default:
                    return true;
            }
        }

        private List<DepthChart>? AddPlayerToDepthChart()
        {

            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            Console.WriteLine("Player Number:");
            var playerNumber = Console.ReadLine();

            Console.WriteLine("Player Name:");
            var playerName = Console.ReadLine();

            Console.WriteLine("Player Position:");
            var playerPosition = Console.ReadLine();

            if (!int.TryParse(playerNumber, out var playerNo))
            {
                throw new Exception();
            }

            var player = new Player
            {
                Name = playerName,
                Number = playerNo
            };

            var depthChartServices = new DepthChartServices();
            return depthChartServices.AddPlayerToDepthChart(_depthChartList, teamName, player, playerPosition);
            
        }
    }
}
