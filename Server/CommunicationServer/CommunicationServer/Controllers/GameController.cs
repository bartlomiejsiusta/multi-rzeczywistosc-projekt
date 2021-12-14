using CommunicationServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        public static Dictionary<Guid, PlayerState> ActivePlayers { get; set; } = new Dictionary<Guid, PlayerState>();
        public static Dictionary<string, GameState> ActiveGames { get; set; } = new Dictionary<string, GameState>();

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

        [HttpPost("Enter")]
        public ActionResult Enter([FromForm] string playerId, [FromForm] string gameId)
        {
            if (ActiveGames.ContainsKey(gameId))
            {
                GameState game = ActiveGames[gameId];

                game.AddGuestPlayer(playerId);

                return Ok(gameId);
            }

            return BadRequest("Game doesn't exist");
        }

        [HttpPost("PostCoordinates")]
        public ActionResult PostCoordinates([FromForm] string coordinate, [FromForm] string gameId, [FromForm] Guid playerId)
        {
            try
            {
                var game = ActiveGames[gameId];
                var type = game.GetPlayerType(playerId);

                game.PerformMove(type, coordinate);
            }
            catch (GameException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
