using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Puissance4.API.DTO;
using Puissance4.Business.BusinessObjects;
using Puissance4.Business.Exceptions;
using Puissance4.Business.Services;
using Puissance4.Domain.Enums;
using System.Security.Claims;

namespace Puissance4.API.Hubs
{
    public class GameHub(P4Service p4Service, GameService gameService): Hub
    {

        [Authorize]
        public async Task CreateGame(CreateGameDTO dto)
        {

            GameBO g = gameService.CreateGame(dto.Color, dto.VersusAI, UserId);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, g.Id.ToString());

            if(dto.VersusAI)
            {
                g.VersusAI = true;
                if(dto.Color == P4Color.Yellow)
                {
                    p4Service.AIPlay(g.Grid, P4Color.Red, 4);
                }
            }


            await Clients.Caller.SendAsync("currentGame", new GameDTO(g, true));
            await BroadCastAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task JoinGame(GameIdDTO dto)
        {
            GameBO g = gameService.JoinGame(dto.GameId, UserId);

            await Groups.AddToGroupAsync(Context.ConnectionId, dto.GameId.ToString());
            await Clients.Group(dto.GameId.ToString())
                .SendAsync("currentGame", new GameDTO(g, true));
            await BroadCastAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task Play(PlayDTO dto)
        {
            GameBO? game = gameService.Find(dto.GameId);
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

            try
            {
                int playerY = p4Service.Play(game.Grid, dto.X, color).Item2;
                await Clients.Group(game.Id.ToString()).SendAsync("move", new MoveDTO(dto.X, playerY, color));

                if (game.VersusAI && game.Winner == null)
                {
                    (int, int) opponentMove = p4Service.AIPlay(game.Grid, color.Switch(), 4);
                    await Clients.Caller.SendAsync(
                        "move", new MoveDTO(opponentMove.Item1, opponentMove.Item2, color.Switch())
                    );
                }
                if(game.Winner != null)
                {
                    await Clients.Group(game.Id.ToString()).SendAsync("currentGame", new GameDTO(game, true));
                }
            } 
            catch (GameException ex) 
            {
                await Clients.Caller.SendAsync("error", ex.Message);
            }
        }

        [Authorize]
        public async Task LeaveGame(GameIdDTO dto)
        {
            GameBO game = gameService.Leave(dto.GameId, UserId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, dto.GameId.ToString());
            await Clients.Caller.SendAsync("leave");
            await Clients.OthersInGroup(dto.GameId.ToString()).SendAsync("opponentLeave");
            await Clients.OthersInGroup(dto.GameId.ToString()).SendAsync("currentGame", new GameDTO(game, true));

            if(!(game.YellowPlayerConnected ?? false) && !(game.RedPlayerConnected ?? false))
            {
                gameService.Remove(game);
                await BroadCastAllGamesAsync(Clients.All);
            }
        }

        public async override Task OnConnectedAsync()
        {
            await BroadCastAllGamesAsync(Clients.Caller);
            GameBO? game = gameService.FindByPlayerId(UserId);
            if (game != null)
            {
                await Clients.Caller.SendAsync("currentGame", new GameDTO(game, true));
            }
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            await BroadCastAllGamesAsync(Clients.All);
            GameBO? game = gameService.FindByPlayerId(UserId);
            if (game != null)
            {
                if(game.Winner != null || game.VersusAI)
                {
                    await LeaveGame(new GameIdDTO() { GameId = game.Id });
                }
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
