using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CommunicationServer.Controllers.GameState;

namespace CommunicationServer.Structures
{
    public class ShotResult
    {
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>
        /// Status pola o koordynatach X i Y po wykonaniu strzału
        /// </summary>
        public StateOfCoordinate CurrentStateOfMapPosition { get; set; }
    }
}
