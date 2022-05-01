
namespace FD.Library.Models
{
    public class DepthChart
    {
        public DepthChart()
        {
            Team = new Team();
            DepthChartEntities = new List<DepthChartEntity>();
            LastUpdate = DateTime.Now;
        }

        public Team Team { get; set; }
        public List<DepthChartEntity> DepthChartEntities { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class DepthChartEntity
    {
        public string Position { get; set; } = string.Empty;
        public List<DepthChartPlayerRel> PositionPlayers { get; set; } = new();
    }
}
