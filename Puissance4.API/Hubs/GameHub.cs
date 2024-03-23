using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Puissance4.API.DTO;
using Puissance4.Business.BusinessObjects;
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
            GameBO game = gameService.CreateGame(dto.Color, dto.VersusAI, UserId);
            if(dto.VersusAI)
            {
                game.VersusAI = true;
                if(dto.Color == P4Color.Yellow)
                {
                    p4Service.AIPlay(game.Grid, P4Color.Red, 4);
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);
            await SendCurrentGameAsync(Clients.Caller, game);
            await SendAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task JoinGame(GameIdDTO dto)
        {
            GameBO game = gameService.JoinGame(dto.GameId, UserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);
            await SendCurrentGameAsync(Clients.Group(game.Name), game);
            await SendInfoAsync(Clients.OthersInGroup(game.Name), "Your opponent join the game");
            await SendAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task Play(PlayDTO dto)
        {
            GameBO? game = gameService.Find(dto.GameId);
            if (game is null)
            {
                return; 
            }
            P4Color color = game.YellowPlayerId == UserId 
                ? P4Color.Yellow
                : game.RedPlayerId == UserId 
                    ? P4Color.Red 
                    : P4Color.None;

            p4Service.Play(game.Grid, dto.X, color);
            await SendCurrentGameAsync(Clients.Group(game.Name), game);

            if (game.VersusAI && game.Winner == null)
            {
                p4Service.AIPlay(game.Grid, color.Switch(), 4);
                await SendCurrentGameAsync(Clients.Group(game.Name), game);
            }

            if(game.Winner != null)
            {
                gameService.Remove(game);
                await SendAllGamesAsync(Clients.All);
            }
        }

        [Authorize]
        public async Task LeaveGame(GameIdDTO dto)
        {
            GameBO game = gameService.Leave(dto.GameId, UserId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Name);
            await SendCurrentGameAsync(Clients.Group(game.Name), game);
            await SendInfoAsync(Clients.OthersInGroup(game.Name), "Your opponent has left the game");
            await SendAllGamesAsync(Clients.All);
        }

        [Authorize]
        public async Task ClaimVictory(GameIdDTO dto)
        {
            GameBO game = gameService.ClaimVictory(dto.GameId, UserId);
            await SendCurrentGameAsync(Clients.Caller, game);
            await SendAllGamesAsync(Clients.All);
        }

        public async override Task OnConnectedAsync()
        {
            await SendInfoAsync(Clients.Caller, "Reconnected");
            GameBO? game = gameService.FindByPlayerId(UserId);
            if (game != null)
            {
                gameService.Reconnect(game.Id, UserId);
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);
                await SendCurrentGameAsync(Clients.Group(game.Name), game);
                await SendInfoAsync(Clients.OthersInGroup(game.Name), "Your opponent is reconnected");
            }
            await SendAllGamesAsync(Clients.Caller);
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            GameBO? game = gameService.FindByPlayerId(UserId);
            if(game != null)
            {
                gameService.Disconnect(game.Id, UserId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Name);
                await SendCurrentGameAsync(Clients.Group(game.Name), game);
                await SendInfoAsync(Clients.OthersInGroup(game.Name), "Your opponent is disconnected");
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

        private async Task SendAllGamesAsync(IClientProxy clientProxy)
        {
            await clientProxy.SendAsync("allGames", gameService.FindAll().Select(g => new GameDTO(g)));
        }

        private async Task SendCurrentGameAsync(IClientProxy clientProxy, GameBO? game)
        {
            await clientProxy.SendAsync("currentGame", game == null ? null : new GameDTO(game, true));
        }

        private async Task SendInfoAsync(IClientProxy clientProxy, string message)
        {
            await clientProxy.SendAsync("info", message);
        }

        private async Task SendErrorAsync(IClientProxy clientProxy, string message)
        {
            await clientProxy.SendAsync("error", message);
        }
    }
}
