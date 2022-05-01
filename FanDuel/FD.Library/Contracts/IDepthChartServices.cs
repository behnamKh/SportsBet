
using FD.Library.Models;

namespace FD.Library.Contracts
{
    public interface IDepthChartServices
    {
        void AddPlayerToDepthChart(string teamName, Player player, string position, int? positionDepth = null);
        Player? RemovePlayerFromDepthChart(string teamName, Player? player, string position);
        List<DepthChartPlayerRel>? GetBackups(string teamName, Player player, string position);
        List<DepthChart>? GetFullDepthChart(string teamName);
    }
}
