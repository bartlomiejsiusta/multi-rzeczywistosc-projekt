using CommunicationServer.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommunicationServer.Controllers
{
    public class GameState
    {
        public Guid? HostPlayerId { get; set; }
        public Guid? GuestPlayerId { get; set; }

        public GameStates CurrentGameState { get; set; } = GameStates.PlacingShips;

        public const int MAP_SIZE = 10;

        public enum GameStates
        {
            PlacingShips,
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
            Empty = 0,
            EmptyShot = 1,
            Ship = 2,
            Wreck = 3
        }

        /// <summary>
        /// Pierwszy wymiar to X, drugi - Y
        /// </summary>
        public StateOfCoordinate[][] HostMap;
        /// <summary>
        /// Pierwszy wymiar to X, drugi - Y
        /// </summary>
        public StateOfCoordinate[][] GuestMap;

        public Dictionary<int, int> HostShipsToPlace = new Dictionary<int, int> {
            {2, 2 },
            {3, 2 },
            {4, 2 },
        };

        public Dictionary<int, int> GuestShipsToPlace = new Dictionary<int, int> {
            {2, 2 },
            {3, 2 },
            {4, 2 },
        };

        public GameState()
        {
            HostMap = new StateOfCoordinate[MAP_SIZE][];
            for (var h = 0; h < MAP_SIZE; h++)
            {
                HostMap[h] = new StateOfCoordinate[MAP_SIZE];
            }

            GuestMap = new StateOfCoordinate[MAP_SIZE][];
            for (var h = 0; h < MAP_SIZE; h++)
            {
                GuestMap[h] = new StateOfCoordinate[MAP_SIZE];
            }
        }

        private int _movesLeft = MAP_SIZE * MAP_SIZE;
        public int MovesLeft
        {
            get
            {
                return _movesLeft;
            }
        }

        public void AddPlayer(string playerId)
        {
            if (HostPlayerId == null)
            {
                HostPlayerId = Guid.Parse(playerId);
            }
            else
            {
                if (HostPlayerId == Guid.Parse(playerId))
                {
                    throw new GameException("This player is already added");
                }

                if (GuestPlayerId == null)
                {
                    GuestPlayerId = Guid.Parse(playerId);
                }
                else
                {
                    throw new GameException("Max number of players exceeded");
                }
            }
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

        private void Shoot(StateOfCoordinate[][] map, int firstCoord, int secondCoord)
        {
            if (map[firstCoord][secondCoord] == StateOfCoordinate.Empty)
            {
                map[firstCoord][secondCoord] = StateOfCoordinate.EmptyShot;
            }
            else if (map[firstCoord][secondCoord] == StateOfCoordinate.Ship)
            {
                map[firstCoord][secondCoord] = StateOfCoordinate.Wreck;
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

        public void PlaceShip(PlayerType playerType, string coordinates, int shipSize)
        {
            (char letter, int secondCoord) = CoordinatesConverter(coordinates);

            int firstCoord = LetterToNumberEquivalent(letter);

            if (CurrentGameState == GameStates.PlacingShips)
            {
                if (playerType == PlayerType.Host)
                {
                    PlaceShipForUser(HostShipsToPlace, HostMap, shipSize, secondCoord, firstCoord);
                }
                else
                {
                    // Guest
                    PlaceShipForUser(GuestShipsToPlace, GuestMap, shipSize, secondCoord, firstCoord);
                }
            }
            else
            {
                throw new GameException("Game is not in PlacingShips state");
            }
        }

        private static void PlaceShipForUser(Dictionary<int,int> availableOptions, StateOfCoordinate[][] map, int shipSize, int secondCoord, int firstCoord)
        {
            if (availableOptions.TryGetValue(shipSize, out int remaining))
            {
                if (remaining == 0)
                {
                    throw new GameException($"All ships with size {shipSize} are placed.");
                }

                if (TryPlaceShipOnMap(map, (firstCoord, secondCoord), shipSize))
                {
                    remaining--;
                    availableOptions[shipSize] = remaining;
                }
                else
                {
                    throw new GameException($"Placing ship refused. Ship collision or out of map.");
                }
            }
            else
            {
                throw new GameException("Illegal ship size. 2, 3 and 4 available");
            }
        }

        public static bool TryPlaceShipOnMap(StateOfCoordinate[][] map, (int x, int y) coords, int shipSize)
        {

            for (var movement = 0; movement < shipSize; movement++)
            {
                if (coords.y + movement < 0 || coords.y + movement > MAP_SIZE - 1)
                {
                    return false;
                }

                if (map[coords.x][coords.y + movement] != StateOfCoordinate.Empty)
                {
                    return false;
                }
            }

            for (var movement = 0; movement < shipSize; movement++)
            {
                map[coords.x][coords.y + movement] = StateOfCoordinate.Ship;
            }

            return true;
        }

        public PlayerType GetPlayerType(Guid playerId)
        {
            if (playerId == HostPlayerId)
            {
                return PlayerType.Host;
            }
            else if (playerId == GuestPlayerId)
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