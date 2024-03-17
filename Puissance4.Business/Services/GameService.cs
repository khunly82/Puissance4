using Puissance4.Business.Exceptions;
using Puissance4.Domain.Entities;
using Puissance4.Domain.Enums;

namespace Puissance4.Business.Services
{
    public class GameService
    {
        private static List<Game> _games = new();

        public Game CreateGame(P4Color color, bool versusAI, int? userId)
        {
            Guid guid = Guid.NewGuid();

            Game game = new Game
            {
                Id = guid,
                VersusAI = versusAI,
                YellowPlayerId = color == P4Color.Yellow ? userId : null,
                RedPlayerId = color == P4Color.Red ? userId : null,
            };
            _games.Add(game);
            return game;
        }

        public void JoinGame(Guid gameId, int userId)
        {
            Game? game = Find(gameId);
            if (game is null)
            {
                throw new GameException("This game does not exist");
            }
            if (game.VersusAI || (game.YellowPlayerId != null && game.RedPlayerId != null))
            {
                throw new GameException("This place is already taken");
            }
            game.RedPlayerId = game.RedPlayerId == null ? userId : game.RedPlayerId;
            game.YellowPlayerId = game.YellowPlayerId == null ? userId : game.YellowPlayerId;
        }


        public Game? Find(Guid gameId)
        {
            return _games.FirstOrDefault(g => g.Id == gameId);
        }

        public List<Game> FindAll()
        {
            return _games;
        }
    }
}
