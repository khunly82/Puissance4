using Microsoft.AspNetCore.Mvc;
using Puissance4.API.DTO;
using Puissance4.Business.Services;
using Puissance4.Domain.Entities;
using Puissance4.Infrastructure.Security;

namespace Puissance4.API.Controllers
{
    [ApiController]
    public class AuthController(TokenManager tokenManager, PlayerService playerService) : ControllerBase
    {

        [HttpPost("api/Login")]
        public ActionResult Login([FromBody] LoginDTO dto)
        {
            try
            {
                Player player = playerService.Login(dto.Username, dto.Password);
                return Ok(
                    new
                    {
                        player.Id,
                        player.Username,
                        Token = tokenManager.CreateToken(player.Id, player.Username)
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("api/Register")]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            try
            {
                playerService.Register(dto.Username, dto.Password);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
