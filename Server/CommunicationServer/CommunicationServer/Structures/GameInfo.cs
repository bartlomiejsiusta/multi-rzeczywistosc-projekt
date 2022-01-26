using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CommunicationServer.Controllers.GameState;

namespace CommunicationServer.Structures
{
    public class GameInfo
    {
        /// <summary>
        /// Na mapie: indeks 0
        /// </summary>
        public Guid? HostPlayerId { get; set; }
        /// <summary>
        /// Na mapie: indeks 1
        /// </summary>
        public Guid? GuestPlayerId { get; set; }
        public GameStates GameState { get; set; }
        public int MapSize { get; set; }
        public Dictionary<int, int> HostShipsToPlace { get; set; }
        public Dictionary<int, int> GuestShipsToPlace { get; set; }
        public int MovesLeft { get; set; }
    }
}
