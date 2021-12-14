using CommunicationServer.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace CommunicationServer.Controllers
{
    public class GameState
    {
        public Guid HostPlayerId { get; set; }
        public Guid GuestPlayerId { get; set; }

        public GameStates CurrentGameState { get; set; }

        public const int MAP_SIZE = 10;

        public enum GameStates
        {
            HostTurn,
            GuestTurn
        }

        public enum PlayerType
        {
            Host,
            Guest
        }

        public enum StateOfCoordinate
        {
            Empty,
            EmptyShot,
            Ship,
            Wreck
        }

        StateOfCoordinate[,] HostMap = new StateOfCoordinate[MAP_SIZE, MAP_SIZE];
        StateOfCoordinate[,] GuestMap = new StateOfCoordinate[MAP_SIZE, MAP_SIZE];

        private int _movesLeft = MAP_SIZE * MAP_SIZE;
        public int MovesLeft
        {
            get
            {
                return _movesLeft;
            }
        }


        public void AddGuestPlayer(string playerId)
        {
            if (GuestPlayerId != null)
            {
                throw new GameException("Max number of players exceeded");
            }

            GuestPlayerId = playerId;
        }

        public static (char, int) CoordinatesConverter(string coordinates)
        {
            var pattern = @"(\w)(\d+)";
            var matches = Regex.Matches(coordinates, pattern);

            char letterCoord;
            int numberCoord;

            if (matches.Count == 1 && matches[0].Groups.Count == 3)
            {
                letterCoord = matches[0].Groups[1].Value.ToCharArray()[0];
                numberCoord = int.Parse(matches[0].Groups[2].Value);
            }
            else
            {
                throw new GameException("Invalid coordinates");
            }

            return (letterCoord, numberCoord);
        }

        public static int LetterToNumberEquivalent(char letter)
        {
            char upper = char.ToUpper(letter);
            if (upper < 'A' || upper > 'Z')
            {
                throw new ArgumentOutOfRangeException("letter", "Letter out of range");
            }

            return upper - 'A';
        }

        private void Shoot(StateOfCoordinate[,] map, int firstCoord, int secondCoord)
        {
            if (map[firstCoord, secondCoord] == StateOfCoordinate.Empty)
            {
                map[firstCoord, secondCoord] = StateOfCoordinate.EmptyShot;
            }
            else if (map[firstCoord, secondCoord] == StateOfCoordinate.Ship)
            {
                map[firstCoord, secondCoord] = StateOfCoordinate.Wreck;
            }
            else
            {
                throw new GameException("Illegal shot");
            }

            _movesLeft--;
        }

        public void PerformMove(PlayerType playerType, string coordinates)
        {
            (char letter, int secondCoord) = CoordinatesConverter(coordinates);

            int firstCoord = LetterToNumberEquivalent(letter);

            if (CurrentGameState == GameStates.HostTurn && playerType == PlayerType.Host)
            {
                Shoot(GuestMap, firstCoord, secondCoord);
                CurrentGameState = GameStates.GuestTurn;
            }
            else if (CurrentGameState == GameStates.GuestTurn && playerType == PlayerType.Guest)
            {
                Shoot(HostMap, firstCoord, secondCoord);
                CurrentGameState = GameStates.HostTurn;
            }
            else
            {
                throw new GameException("Not your turn");
            }
        }

        public PlayerType GetPlayerType(Guid playerId)
        {
            if(playerId == HostPlayerId)
            {
                return PlayerType.Host;
            }
            else if(playerId == GuestPlayerId)
            {
                return PlayerType.Guest;
            }
            else
            {
                throw new GameException("No such player");
            }
        }
    }
}