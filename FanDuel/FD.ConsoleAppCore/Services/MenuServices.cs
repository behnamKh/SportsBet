using System;
using System.Collections.Generic;
using System.Linq;

using FD.ConsoleAppCore.Contracts;
using FD.ConsoleAppCore.Models;
using FD.Library.Contracts;
using FD.Library.Models;

namespace FD.ConsoleAppCore.Services
{
    public class MenuServices : IMenuServices
    {
        private readonly IDepthChartServices _depthChartServices;

        private readonly List<TeamPlayers> _teamPlayers;

        public MenuServices(IDepthChartServices depthChartServices)
        {
            _depthChartServices = depthChartServices;
            _teamPlayers = new List<TeamPlayers>();
        }

        public bool MainMenu()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1) Add Player");
                Console.WriteLine("2) Display Player List");
                Console.WriteLine("3) Add Player To Depth Chart");
                Console.WriteLine("4) Get Backups");
                Console.WriteLine("5) Get Full Depth Chart");
                Console.WriteLine("6) Remove Player From Depth Chart");
                Console.WriteLine("7) Exit");
                Console.Write("\r\nSelect an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddPlayer();
                        return true;
                    case "2":
                        DisplayPlayerList();
                        return true;
                    case "3":
                        AddPlayerToDepthChart();
                        return true;
                    case "4":
                        GetBackups();
                        return true;
                    case "5":
                        GetFullDepthChart();
                        return true;
                    case "6":
                        RemovePlayerFromDepthChart();
                        return true;
                    case "7":
                        return false;
                    default:
                        return true;
                }
            }
            catch
            {
                return true;
            }
        }

        private void DisplayPlayerList()
        {
            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            var teamPlayers = _teamPlayers.FirstOrDefault(tp => tp.TeamName == teamName);
            if (teamPlayers == null)
            {
                return;
            }

            foreach (var player in teamPlayers.PlayersList)
            {
                Console.WriteLine($"#{player.Number} - {player.Name}");
                Console.WriteLine("-----------------------------");
            }

            Console.WriteLine("Please any keys to continue");
            Console.ReadLine();
        }

        private void AddPlayer()
        {
            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            Console.WriteLine("Player Number:");
            var playerNumber = Console.ReadLine();

            Console.WriteLine("Player Name:");
            var playerName = Console.ReadLine();

            if (!int.TryParse(playerNumber, out var playerNo))
            {
                throw new Exception();
            }

            var teamPlayers = new TeamPlayers();

            var team = _teamPlayers.FirstOrDefault(tp => tp.TeamName == teamName);
            if (team == null)
            {
                teamPlayers.TeamName = teamName;
                teamPlayers.PlayersList = new List<Player>
                {
                    new Player
                    {
                        Name = playerName,
                        Number = playerNo
                    }
                };

                _teamPlayers.Add(teamPlayers);

                return;
            }

            // Add new player into existing list
            teamPlayers.PlayersList = team.PlayersList;
            teamPlayers.PlayersList.Add(new Player
            {
                Name = playerName,
                Number = playerNo
            });

            _teamPlayers.Add(teamPlayers);

        }

        private void AddPlayerToDepthChart()
        {

            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            Console.WriteLine("Player Name:");
            var playerName = Console.ReadLine();

            Console.WriteLine("Player Position:");
            var playerPosition = Console.ReadLine();

            Console.WriteLine("Player Position Depth:");
            var positionDepth = Console.ReadLine();

            if (teamName == null || playerPosition == null) return;

            var player = _teamPlayers.FirstOrDefault(tp => tp.TeamName == teamName)
                ?.PlayersList.FirstOrDefault(p =>
                    string.Equals(p.Name, playerName, StringComparison.CurrentCultureIgnoreCase));

            if (player == null) return;

            _depthChartServices.AddPlayerToDepthChart(teamName, player, playerPosition,
                ParseToNullableInt(positionDepth));

        }

        private void GetFullDepthChart()
        {
            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            if (teamName == null) return;

            var depthChartList = _depthChartServices.GetFullDepthChart(teamName);
            if (depthChartList != null)
            {
                var teamDepthChart =
                    (from d in depthChartList.FirstOrDefault(dc => dc.Team.Name == teamName)?.DepthChartEntities
                     group d by d.Position
                        into g
                     select new { Position = g.Key, Players = g.Select(depthChartEntity => depthChartEntity.PositionPlayers).ToList() });

                foreach (var depthChart in teamDepthChart)
                {
                    var outputPrint = $"{depthChart.Position} - ";
                    outputPrint = depthChart.Players.SelectMany(players => players)
                        .Aggregate(outputPrint, (current, depthChartPlayerRel) => current + $"(#{depthChartPlayerRel.Player?.Number}, {depthChartPlayerRel.Player?.Name}), ");

                    Console.WriteLine(outputPrint.TrimEnd().TrimEnd(','));
                    Console.WriteLine("--------------------");
                }
            }
            else
            {
                Console.WriteLine($"there is no Depth Chart for team {teamName}");
            }

            Console.WriteLine("Please any keys to continue");
            Console.ReadLine();
        }

        private void GetBackups()
        {
            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            Console.WriteLine("Player Position:");
            var playerPosition = Console.ReadLine();

            Console.WriteLine("Player Name:");
            var playerName = Console.ReadLine();

            if (teamName == null || playerPosition == null) return;

            var player = _teamPlayers.FirstOrDefault(tp => tp.TeamName == teamName)
                ?.PlayersList.FirstOrDefault(p =>
                    string.Equals(p.Name, playerName, StringComparison.CurrentCultureIgnoreCase));

            if (player == null) return;

            var players = _depthChartServices.GetBackups(teamName, player, playerPosition);

            if (players != null && !players.Any())
            {
                Console.WriteLine("<NO LIST>");

                Console.WriteLine("Please any keys to continue");
                Console.ReadLine();
            }

            if (players != null)
            {
                foreach (var playerRel in players.Where(playerRel => playerRel.Player != null))
                {
                    Console.WriteLine($"#{playerRel.Player?.Number} - {playerRel.Player?.Name}");
                }
            }
            
            Console.WriteLine("Please any keys to continue");
            Console.ReadLine();
        }

        private void RemovePlayerFromDepthChart()
        {
            Console.WriteLine("Please Enter the Team Name:");
            var teamName = Console.ReadLine();

            Console.WriteLine("Player Position:");
            var playerPosition = Console.ReadLine();

            Console.WriteLine("Player Name:");
            var playerName = Console.ReadLine();

            if (teamName == null || playerPosition == null || playerName == null) return;

            var depthChartList = _depthChartServices.GetFullDepthChart(teamName);
            var player = depthChartList?.FirstOrDefault(tp => tp.Team.Name == teamName)
                ?.DepthChartEntities.FirstOrDefault(dec =>
                    dec.Position == playerPosition)
                ?.PositionPlayers.FirstOrDefault(pp => pp.Player?.Name == playerName)
                ?.Player;

            if (player == null)
            {
                Console.WriteLine("<No Player Found>");
                Console.ReadLine();
                return;
            }

            _depthChartServices.RemovePlayerFromDepthChart(teamName, player, playerPosition);
            Console.WriteLine($"#{player.Number} - {player.Name}");
            Console.WriteLine("Removed Successfully.");


            Console.WriteLine("Please any keys to continue");
            Console.ReadLine();

        }
        private static int? ParseToNullableInt(string id)
        {
            if (id != null)
            {
                return int.TryParse(id, out var result) ? (int?)result : null;
            }

            return null;
        }
    }
}


