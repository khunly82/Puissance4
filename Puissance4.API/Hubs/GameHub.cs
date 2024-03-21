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

            if(dto.VersusAI && dto.Color == P4Color.Yellow)
            {
                p4Service.AIPlay(g.Grid, P4Color.Red, 4);
            }

            await Clients.Caller.SendAsync("currentGame", new GameDTO(g, true));
            await BroadCastAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task JoinGame(JoinGameDTO dto)
        {
            Game g = gameService.JoinGame(dto.GameId, UserId);

            await Groups.AddToGroupAsync(Context.ConnectionId, dto.GameId.ToString());
            await Clients.Group(dto.GameId.ToString()).SendAsync("currentGame", new GameDTO(g, true));
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

            P4Color color = 
                game.YellowPlayerId == UserId 
                    ? P4Color.Yellow
                    : game.RedPlayerId == UserId 
                        ? P4Color.Red : P4Color.None;

            if(color == P4Color.None)
            {
                return;
            }

            int playerY = p4Service.Play(game.Grid, dto.X, color).Item2;

            await Clients.Group(game.Id.ToString()).SendAsync("move", new MoveDTO(dto.X, playerY, color));

            if (game.VersusAI && game.Winner == null)
            {
                (int, int) opponentMove = p4Service.AIPlay(game.Grid, color.Switch(), 1);
                await Clients.Caller.SendAsync(
                    "move", new MoveDTO(opponentMove.Item1, opponentMove.Item2, color.Switch())
                );
            }

            if(game.Winner != null)
            {
                gameService.Remove(game.Id);
                await BroadCastAllGamesAsync(Clients.All);
                await Clients.Group(game.Id.ToString()).SendAsync("gameEnd");
            }
        }

        public async override Task OnConnectedAsync()
        {
            await BroadCastAllGamesAsync(Clients.Caller);
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            gameService.Delete(UserId);
            await BroadCastAllGamesAsync(Clients.All);
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
