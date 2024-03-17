using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Puissance4.API.DTO;
using Puissance4.Business.Services;
using Puissance4.Domain.Entities;
using Puissance4.Domain.Enums;
using System.Security.Claims;

namespace Puissance4.API.Hubs
{
    public class GameHub(P4Service p4Service, GameService gameService): Hub
    {
        

        [Authorize]
        public async Task CreateGame(CreateGameDTO dto)
        {

            Game g = gameService.CreateGame(dto.Color, dto.VersusAI, UserId);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, g.Id.ToString());
            if(dto.VersusAI)
            {
                await BroadCastAllGamesAsync(Clients.Caller);
            }
            await BroadCastAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task JoinGame(JoinGameDTO dto)
        {
            gameService.JoinGame(dto.GameId, (int)UserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, dto.GameId.ToString());

            await BroadCastAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task Play(PlayDTO dto)
        {
            Game? game = gameService.Find(dto.GameId);
            if (game is null)
            {
                return;
            }
            int playerY = p4Service.Play(game.Grid, dto.X, dto.Color).Item2;
            await Clients.Caller.SendAsync("playerMove", new MoveDTO(dto.X,playerY));

            if (game.VersusAI)
            {
                (int, int) opponentMove = p4Service.AIPlay(game.Grid, (P4Color)((int)dto.Color * -1), 4);
                await Clients.Caller.SendAsync("opponentMove", new MoveDTO(opponentMove.Item1, opponentMove.Item2));
            }
            else
            {

            }

        }

        private int? UserId {
            get {
                string? val = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(val is null)
                {
                    return null;
                }
                return int.Parse(val);
            }
        }

        

        private async Task BroadCastAllGamesAsync(IClientProxy clientProxy)
        {
            await clientProxy.SendAsync("allGames", gameService.FindAll().Select(g => new GameDTO(g)));
        }
    }
}
