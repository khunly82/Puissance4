using Puissance4.Business.Exceptions;
using Puissance4.DAL.Repositories;
using Puissance4.Domain.Entities;
using Puissance4.Domain.Enums;

namespace Puissance4.Business.Services
{
    public class GameService(PlayerRepository _playerRepository)
    {
        private static List<Game> _games = new();

        public Game CreateGame(P4Color color, bool versusAI, int? userId)
        {
            Player? player = _playerRepository.FindById(userId);

            if (player is null)
            {
                throw new ArgumentException();
            }

            Guid guid = Guid.NewGuid();

            Game game = new Game
            {
                Id = guid,
                VersusAI = versusAI,
                RedPlayerId = color == P4Color.Red ? userId : null,
                YellowPlayerId = color == P4Color.Yellow ? userId : null,
                RedPlayer = color == P4Color.Red ? player : null,
                YellowPlayer = color == P4Color.Yellow ? player : null,
            };

            _games.Add(game);
            return game;
        }

        public Game JoinGame(Guid gameId, int? userId)
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
            if(game.YellowPlayerId == userId || game.RedPlayerId == userId)
            {
                throw new GameException("You are alreary in this game");
            }

            Player? player = _playerRepository.FindById(userId);

            if (player is null)
            {
                throw new ArgumentException();
            }

            game.RedPlayerId = game.RedPlayerId == null ? userId : game.RedPlayerId;
            game.YellowPlayerId = game.YellowPlayerId == null ? userId : game.YellowPlayerId;
            if(game.RedPlayerId == null)
            {
                game.RedPlayerId = userId;
                game.RedPlayer = player;
            } 
            else
            {
                game.YellowPlayerId = userId;
                game.YellowPlayer = player;
            }

            return game;
        }


        public Game? Find(Guid gameId)
        {
            return _games.FirstOrDefault(g => g.Id == gameId);
        }

        public List<Game> FindAll()
        {
            return _games;
        }

        public void Delete(int? userId)
        {
            Game? g = _games.FirstOrDefault(g => g.YellowPlayerId == userId || g.RedPlayerId == userId);
            if(g != null)
            {
                _games.Remove(g);
            }
        }

        public void Remove(Guid id)
        {
            Game? g = _games.FirstOrDefault(ga => ga.Id == id);
            if(g != null)
            {
                _games.Remove(g);
            }
        }
    }
}
