using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using FD.Library.Models;
using FD.Library.Services;
using FD.Library.Contracts;
using FD.Library.Repositories;

namespace FD.Library.Tests
{
    [TestClass]
    public class DepthChartServicesTests
    {
        private readonly IDepthChartServices _service;
        private readonly Player _player1 = new Player { Name = "Tom Brady", Number = 12 };
        private readonly Player _player2 = new Player { Name = "Blaine Gabbert", Number = 11 };
        private readonly Player _player3 = new Player { Name = "Kyle Trask", Number = 2 };
        private readonly Player _player4 = new Player { Name = "Mike Evans", Number = 13 };
        private readonly Player _player5 = new Player { Name = "Jaelon Darden", Number = 1 };
        private readonly Player _player6 = new Player { Name = "Scott Miller", Number = 10 };
        private readonly string _teamName = "Tampa Bay Buccaneers";

        public DepthChartServicesTests()
        {
            _service = new DepthChartServices();
        }
        

        [TestMethod]
        public void AddPlayerToDepthChart_AddPlayerToNoneExistingTeamDepthChart_Successfull()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";
            
            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);
            var result = _service.GetFullDepthChart(_teamName);
            var player1 = result
               .FirstOrDefault(dc => dc.Team.Name == _teamName).DepthChartEntities
               .FirstOrDefault(dce => dce.Position == position).PositionPlayers
               .FirstOrDefault(pp => pp.Player.Name == _player1.Name && _player1.Number == _player1.Number).Player;
            
            Assert.AreEqual(player1.Name, _player1.Name);
            Assert.AreEqual(player1.Number, _player1.Number);

        }

        [TestMethod]
        public void AddPlayerToDepthChart_AddPlayerDuplicatedPlayerTeamDepthChart_ShouldNotBeAdded()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";

            // First attempt
            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);
            // Second attempt
            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);

            var result = _service.GetFullDepthChart(_teamName);

            var playersInChart = result
                .FirstOrDefault(dc => dc.Team.Name == _teamName).DepthChartEntities
                .FirstOrDefault(dce => dce.Position == position).PositionPlayers.ToList();
                
            Assert.AreEqual(1, playersInChart.Count);

        }

        [TestMethod]
        public void AddPlayerToDepthChart_AddingPlayerWithoutPositionDepth_ShouldBeAddedToTheEndOfList()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";

            // First Player
            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);
            
            // Second Player
            _service.AddPlayerToDepthChart(_teamName, _player2, position, null);

            var result = _service.GetFullDepthChart(_teamName);
            var player2PositionDepth = result
                .FirstOrDefault(dc => dc.Team.Name == _teamName).DepthChartEntities
                .FirstOrDefault(dce => dce.Position == position).PositionPlayers
                .FirstOrDefault(pp=>pp.Player.Name == _player2.Name && _player2.Number == _player2.Number).PositionDepth;

            Assert.AreEqual(1, player2PositionDepth);

        }

        [TestMethod]
        public void AddPlayerToDepthChart_AddingPlayerWouldGetPriority_ShouldRePositionList()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";

            // First Player
            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);
            
            // Second Player
            _service.AddPlayerToDepthChart(_teamName, _player2, position, 1); 
            
            // Third Player
            _service.AddPlayerToDepthChart(_teamName, _player3, position, 1);

            var result = _service.GetFullDepthChart(_teamName);
            var player2PositionDepth = result
                .FirstOrDefault(dc => dc.Team.Name == _teamName).DepthChartEntities
                .FirstOrDefault(dce => dce.Position == position).PositionPlayers
                .FirstOrDefault(pp => pp.Player.Name == _player2.Name && _player2.Number == _player2.Number).PositionDepth;

            Assert.AreEqual(2, player2PositionDepth);

        }

        [TestMethod]
        public void RemovePlayerFromDepthChart_Successfull()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";

            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);

            _service.RemovePlayerFromDepthChart(_teamName, _player1, position);

            var result = _service.GetFullDepthChart(_teamName);
            var playersInChart = result
                .FirstOrDefault(dc => dc.Team.Name == _teamName).DepthChartEntities
                .FirstOrDefault(dce => dce.Position == position).PositionPlayers.ToList();
            
            Assert.AreEqual(0, playersInChart.Count);
        }

        [TestMethod]
        public void GetBackups_ShouldReturnPlayerBackups()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";
            
            // First Player
            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);

            // Second Player
            _service.AddPlayerToDepthChart(_teamName, _player2, position, 1);

            // Third Player
            _service.AddPlayerToDepthChart(_teamName, _player3, position, 2);

            // Forth Player
            _service.AddPlayerToDepthChart(_teamName, _player4, position, 3);

            // Fifth Player
            _service.AddPlayerToDepthChart(_teamName, _player5, position, 3);

            // Sixth Player
            _service.AddPlayerToDepthChart(_teamName, _player6, position, 3);

            var result = _service.GetBackups(_teamName, _player2, position);

            Assert.AreEqual(4, result.Count);

        }

        [TestMethod]
        public void GetBackups_ShouldReturnNullIfPlayerHasNoBackups()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";

            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);

            var result = _service.GetBackups(_teamName, _player1, position);

            Assert.AreEqual(0, result.Count);

        }

        [TestMethod]
        public void GetBackups_ShouldReturnNullIfPlayerNotListedInDepthChart()
        {
            DepthChartRepository.InitialDepthChart();
            var position = "QB";

            _service.AddPlayerToDepthChart(_teamName, _player1, position, 0);

            var result = _service.GetBackups(_teamName, _player2, position);

            Assert.AreEqual(0, result.Count);

        }


    }
}
