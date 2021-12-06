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

        [HttpPost]
        public ActionResult PostCoordinates([FromForm] string coordinate)
        {
            return Ok(coordinate);
        }
    }
}
