using CommunicationServer.Exceptions;
using CommunicationServer.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CommunicationServer.Controllers.GameState;

namespace CommunicationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        public static Dictionary<Guid, PlayerState> ActivePlayers { get; set; } = new Dictionary<Guid, PlayerState>();
        public static Dictionary<string, GameState> ActiveGames { get; set; } = new Dictionary<string, GameState>();

        public static bool AppSetup = false;

        public GameController()
        {
            // ustanowienie przykładowej gry
            if (!AppSetup)
            {
                ActivePlayers.Add(Guid.Parse("c3784240-bbeb-417f-a413-2cf11b758ae7"), new PlayerState());
                ActivePlayers.Add(Guid.Parse("a29b5c23-793d-4d6d-ab8f-a789579dcbb5"), new PlayerState());
                GameState game = new GameState();
                ActiveGames.Add("abc", game);

                game.AddPlayer("c3784240-bbeb-417f-a413-2cf11b758ae7");
                game.AddPlayer("a29b5c23-793d-4d6d-ab8f-a789579dcbb5");

                game.PlaceShip(PlayerType.Guest, "A2", 2);
                game.PlaceShip(PlayerType.Guest, "B3", 2);
                game.PlaceShip(PlayerType.Guest, "C4", 3);
                game.PlaceShip(PlayerType.Guest, "D5", 3);
                game.PlaceShip(PlayerType.Guest, "E6", 4);
                game.PlaceShip(PlayerType.Guest, "F6", 4);

                game.PlaceShip(PlayerType.Host, "A2", 2);
                game.PlaceShip(PlayerType.Host, "B3", 2);
                game.PlaceShip(PlayerType.Host, "C4", 3);
                game.PlaceShip(PlayerType.Host, "D5", 3);
                game.PlaceShip(PlayerType.Host, "E6", 4);
                game.PlaceShip(PlayerType.Host, "F6", 4);

                AppSetup = true;
            }
        }

        /// <summary>
        /// Rejestracja nowego gracza
        /// </summary>
        /// <param name="claimedId">Guid jako string</param>
        /// <response code="200">Gracz dodany</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Register")]
        public ActionResult RegisterPlayer([FromForm] string claimedId)
        {
            Guid claimedGuid = Guid.Parse(claimedId);

            if (ActivePlayers.ContainsKey(claimedGuid))
            {
                return BadRequest("Id already taken");
            }
            else
            {
                ActivePlayers.Add(claimedGuid, new PlayerState());

                return Ok();
            }
        }

        /// <summary>
        /// Utworzenie nowej gry
        /// </summary>
        /// <response code="200">Zwraca identyfikator utworzonej gry</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Create")]
        public ActionResult Create()
        {
            string newRandomId;
            int attempts = 3;

            do
            {
                newRandomId = Tools.Tools.RandomString(10);
                attempts--;
            }
            while (attempts > 0 && !ActiveGames.TryAdd(newRandomId, new GameState()));

            return Ok(newRandomId);
        }

        /// <summary>
        /// Dołączenie do istniejącej gry. Dwóch graczy może dołączyć do gry.
        /// </summary>
        /// <param name="playerId">Guid dołączającego gracza</param>
        /// <param name="gameId">Identyfikator gry</param>
        /// <response code="200">Identyfikator gry (w przypadku powodzenia)</response>
        /// <response code="400">Gra nie istnieje lub nie można dodać gracza (bo przekroczono maksymalną ilość graczy (2))</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Enter")]
        public ActionResult Enter([FromForm] string playerId, [FromForm] string gameId)
        {
            if (ActiveGames.ContainsKey(gameId))
            {
                GameState game = ActiveGames[gameId];

                try
                {
                    game.AddPlayer(playerId);
                }
                catch (GameException e)
                {
                    return BadRequest($"Problem z dodaniem gracza: {e.Message}");
                }

                return Ok(gameId);
            }

            return BadRequest("Game doesn't exist");
        }

        /// <summary>
        /// Ustawia statek na planszy. Statki będą ustawiane na planszy wartykalnie/pionowo. Podana wartość `coordinate` to dolna część statku
        /// </summary>
        /// <param name="coordinate">Format koordynatów [A-Z][\d+] (litera a potem liczba). Przykładowo: A10 albo B2</param>
        /// <param name="gameId">Identyfikator gry</param>
        /// <param name="playerId">Guid gracza wykonującego ruch</param>
        /// <param name="shipSize">Wielkość statku (2, 3 albo 4)</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("PlaceShip")]
        public ActionResult PlaceShip([FromForm] string coordinate, [FromForm] string gameId, [FromForm] Guid playerId, [FromForm] int shipSize)
        {
            try
            {
                var game = ActiveGames[gameId];
                var type = game.GetPlayerType(playerId);

                game.PlaceShip(type, coordinate, shipSize);
            }
            catch (GameException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Ruch gracza, wykonanie strzału w wybrane pole.
        /// </summary>
        /// <param name="coordinate">Format koordynatów [A-Z][\d+] (litera a potem liczba). Przykładowo: A10 albo B2</param>
        /// <param name="gameId">Identyfikator gry</param>
        /// <param name="playerId">Guid gracza wykonującego ruch</param>
        /// <response code="400">Nieprawidłowy ruch</response>
        /// <response code="200">Ruch zapisany. Zwraca strukturę ShotResult</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("PostCoordinates")]
        public ActionResult<ShotResult> PostCoordinates([FromForm] string coordinate, [FromForm] string gameId, [FromForm] Guid playerId)
        {
            try
            {
                var game = ActiveGames[gameId];
                var type = game.GetPlayerType(playerId);

                ShotResult shotResult = game.PerformMove(type, coordinate);

                return Ok(shotResult);
            }
            catch (GameException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Zwraca dwie tablice stanu gry (plansza gracza 1. i plansza gracza 2.)
        /// </summary>
        /// <param name="gameId">Identyfikator gry</param>
        /// <response code="200">Trójwymiarowa tablica. Pierwszy to rodzaj plaszy: 
        /// 0 - plansza gracza `host`, 1- plansza gracza `guest`, oraz koordynaty x i y wybranej planszy.
        /// Przykładowo: Tablica[0][1][2] to stan pola na planszy gracza `host` na pozycji x = 1 i pozycji y = 2</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("MapState")]
        public ActionResult GetMapState(string gameId)
        {
            var game = ActiveGames[gameId];

            return Ok(new StateOfCoordinate[][][]
            {
                game.HostMap,
                game.GuestMap
            });
        }

        /// <summary>
        /// Zwraca stan gry. Możliwe są trzy stany:
        /// PlacingShips = 0 (ustawianie statków),
        /// HostTurn = 1 (gra, ruch gracza 1),
        /// GuestTurn = 2 (gra, ruch gracza 2),
        /// EndGameHostWon = 3,
        /// EndGameGuestWon = 4
        /// </summary>
        /// <param name="gameId">Identyfikator gry</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("CurrentGameState")]
        public ActionResult CurrentGameState(string gameId)
        {
            var game = ActiveGames[gameId];

            return Ok(game.CurrentGameState);
        }

        /// <summary>
        /// Zwraca informacje o grze
        /// </summary>
        /// <param name="gameId">Identyfikator gry</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("GameInfo")]
        public ActionResult<GameInfo> GameInfo(string gameId)
        {
            var game = ActiveGames[gameId];

            return new GameInfo()
            {
                GameState = game.CurrentGameState,
                GuestShipsToPlace = game.GuestShipsToPlace,
                HostShipsToPlace = game.HostShipsToPlace,
                MapSize = GameState.MAP_SIZE,
                MovesLeft = game.MovesLeft
            };
        }
    }
}
