using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicationServer.Exceptions
{
    public class GameException : Exception
    {
        public GameException(string message)
        : base(message)
        {
        }
    }
}
