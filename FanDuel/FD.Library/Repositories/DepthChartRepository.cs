using FD.Library.Models;

namespace FD.Library.Repositories
{
    public static class DepthChartRepository
    {
        private static List<DepthChart>? _depthChartList = new();

        public static List<DepthChart>? GetDepthChart()
        {
            return _depthChartList;
        }

        public static void UpdateDepthChart(List<DepthChart>? depthChart)
        {
            if (depthChart == null) return;
            _depthChartList = depthChart;
        }

        // Need this for running multiple Unit Tests
        public static void InitialDepthChart()
        {
            _depthChartList = new List<DepthChart>();
        }

    }
}
