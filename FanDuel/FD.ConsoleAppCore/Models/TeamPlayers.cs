using System.Collections.Generic;

using FD.Library.Models;

namespace FD.ConsoleAppCore.Models
{
    public class TeamPlayers
    {
        public string TeamName { get; set; }
        public List<Player> PlayersList { get; set; }
    }
}
