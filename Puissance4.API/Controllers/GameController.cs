using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puissance4.API.DTO;
using Puissance4.Business.Services;

namespace Puissance4.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController(GameService gameService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetByPlayer([FromQuery]int playerId)
        {
            return Ok(gameService.FindByPlayerId(playerId).Select(g => new GameByPlayerDTO(g)));
        }
    }
}
