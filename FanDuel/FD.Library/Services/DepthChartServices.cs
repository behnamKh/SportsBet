
using FD.Library.Contracts;
using FD.Library.Models;
using FD.Library.Repositories;

namespace FD.Library.Services
{
    public class DepthChartServices : IDepthChartServices
    {
        public void AddPlayerToDepthChart(string teamName, Player player, string position, int? positionDepth)
        {
            var depthChartQuery = DepthChartRepository.GetDepthChart()?.FirstOrDefault(dc => dc.Team.Name == teamName);

            if (depthChartQuery == null)
            {
                // Add new Team Depth Chart
                var depthChart = new DepthChart
                {
                    Team = new Team
                    {
                        Name = teamName,
                        Id = Guid.NewGuid()
                    },
                    LastUpdate = DateTime.Now,
                    DepthChartEntities = new List<DepthChartEntity>()
                };


                var positionPlayers = new List<DepthChartPlayerRel> { new DepthChartPlayerRel
                {
                    Player = player,
                    PositionDepth = 0
                } };

                depthChart.DepthChartEntities.Add(new DepthChartEntity()
                {
                    PositionPlayers = positionPlayers,
                    Position = position
                });

                var updatedChart = new List<DepthChart> {depthChart};

                DepthChartRepository.UpdateDepthChart(updatedChart);

            }
            else
            {
                var positionQuery = depthChartQuery.DepthChartEntities
                    .FirstOrDefault(de => de.Position == position);
                var updatedChart = DepthChartRepository.GetDepthChart();
                if (positionQuery != null)
                {
                    var positionPlayer = new DepthChartPlayerRel
                    {
                        Player = player,
                        PositionDepth = positionDepth
                    };
                    
                    var depthChartEntity = updatedChart?.FirstOrDefault(dc => dc.Team.Name == teamName)
                        ?.DepthChartEntities.FirstOrDefault(de => de.Position == position);
                    if (depthChartEntity != null && !IsEntityExisted(depthChartEntity.PositionPlayers, positionPlayer))
                    {
                        depthChartEntity.PositionPlayers =
                            RePositionDepth(depthChartEntity.PositionPlayers, positionPlayer);
                        DepthChartRepository.UpdateDepthChart(updatedChart);
                    }
                }
                else
                {
                    var positionPlayer = new DepthChartPlayerRel
                    {
                        Player = player,
                        PositionDepth = positionDepth
                    };
                    var positionPlayers = new List<DepthChartPlayerRel> { positionPlayer };
                    
                    updatedChart?.FirstOrDefault(dc => dc.Team.Name == teamName)
                        ?.DepthChartEntities.Add(new DepthChartEntity
                        {
                            PositionPlayers = positionPlayers,
                            Position = position
                        });

                }

                var chart = updatedChart?.FirstOrDefault(dc => dc.Team.Name == teamName);
                if (chart != null) chart.LastUpdate = DateTime.Now;
                DepthChartRepository.UpdateDepthChart(updatedChart);
            }
            
        }

        public Player? RemovePlayerFromDepthChart(string teamName, Player? player,
            string position)
        {
            if (DepthChartRepository.GetDepthChart() is {Count: 0})
            {
                return null;
            }
            var updatedChart = DepthChartRepository.GetDepthChart();
            var depthChartQuery = updatedChart?.FirstOrDefault(dc => dc.Team.Name == teamName);

            var playerQuery = depthChartQuery?.DepthChartEntities.FirstOrDefault(dce => dce.Position == position)?.PositionPlayers.FirstOrDefault(pf => pf.Player == player);

            if (playerQuery == null) return null;

            if (depthChartQuery != null) depthChartQuery.LastUpdate = DateTime.Now;
            
            updatedChart?.FirstOrDefault(dc => dc.Team.Name == teamName)
                ?.DepthChartEntities.FirstOrDefault(dce => dce.Position == position)?.PositionPlayers
                .Remove(playerQuery);
            DepthChartRepository.UpdateDepthChart(updatedChart);
            
            return player;
        }

        public List<DepthChartPlayerRel>? GetBackups(string teamName, Player player, string position)
        {
            var players = DepthChartRepository.GetDepthChart()?.FirstOrDefault(dc => dc.Team.Name == teamName)
                ?.DepthChartEntities
                .FirstOrDefault(dce => dce.Position == position)
                ?.PositionPlayers;
            var playerPositionDepth = players?.FirstOrDefault(p => p.Player?.Name == player.Name)?.PositionDepth;

            return players?.Where(dc => dc.PositionDepth > playerPositionDepth).ToList();
        }

        public List<DepthChart>? GetFullDepthChart(string teamName)
        {
            return DepthChartRepository.GetDepthChart();
        }

        private static List<DepthChartPlayerRel> RePositionDepth(List<DepthChartPlayerRel> playerList, DepthChartPlayerRel newPlayer)
        {
            if (!playerList.Any())
            {
                newPlayer.PositionDepth = 0;
                playerList.Add(newPlayer);

                return playerList;
            }
            if (newPlayer.PositionDepth == null)
            {

                var maxPositionDepth = playerList.Max(p => p.PositionDepth);
                newPlayer.PositionDepth = maxPositionDepth + 1;
                playerList.Add(newPlayer);

                return playerList;
            }

            if (playerList.Any(pl => pl.PositionDepth == newPlayer.PositionDepth))
            {
                var max = newPlayer.PositionDepth + 1;
                foreach (var positionPlayer in playerList
                             .Where(p => p.PositionDepth >= newPlayer.PositionDepth))
                {
                    positionPlayer.PositionDepth = max++;
                }
                playerList.Add(newPlayer);
            }
            else
            {
                playerList.Add(newPlayer);
            }
            
            return playerList.OrderBy(pl => pl.PositionDepth).ToList();
        }

        private static bool IsEntityExisted(IEnumerable<DepthChartPlayerRel> playerList, DepthChartPlayerRel newPlayer)
        {
            return  playerList.Any(pl => pl.Player?.Name == newPlayer.Player?.Name && pl.Player?.Number == newPlayer.Player?.Number);
        }

    }
}
