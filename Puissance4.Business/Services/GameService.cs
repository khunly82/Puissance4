﻿using Puissance4.Business.BusinessObjects;
using Puissance4.Business.Enums;
using Puissance4.Business.Exceptions;
using Puissance4.DAL.Repositories;
using Puissance4.Domain.Entities;
using Puissance4.Domain.Enums;

namespace Puissance4.Business.Services
{
    public class GameService(PlayerRepository _playerRepository)
    {
        private static List<GameBO> _games = new();

        public GameBO CreateGame(P4Color color, bool versusAI, int? userId)
        {
            Player? player = _playerRepository.FindById(userId)
                ?? throw new GameException("This player does not exist");

            Guid guid = Guid.NewGuid();

            GameBO game = new()
            {
                Id = guid,
                VersusAI = versusAI,
            };

            JoinGame(game, color, player);

            _games.Add(game);
            return game;
        }

        public GameBO JoinGame(Guid gameId, int? userId)
        {
            GameBO? game = Find(gameId)
                ?? throw new GameException("This game does not exist");

            Player? player = _playerRepository.FindById(userId)
                ?? throw new GameException("This player does not exist");

            if (game.VersusAI || (game.YellowPlayerId != null && game.RedPlayerId != null))
            {
                throw new GameException("This place is already taken");
            }
            if(game.YellowPlayerId == userId || game.RedPlayerId == userId)
            {
                throw new GameException("You are alreary in this game");
            }

            if (game.RedPlayerId == null)
            {
                JoinGame(game, P4Color.Red, player);
            } 
            else
            {
                JoinGame(game, P4Color.Yellow, player);
            }

            return game;
        }

        private void JoinGame(GameBO game, P4Color color, Player player)
        {
            if (color == P4Color.Red)
            {
                game.RedPlayerId = player.Id;
                game.RedPlayerName = player.Username;
                game.RedPlayerStatus = PlayerStatus.Connected;
            }
            if (color == P4Color.Yellow)
            {
                game.YellowPlayerId = player.Id;
                game.YellowPlayerName = player.Username;
                game.YellowPlayerStatus = PlayerStatus.Connected;
            }
        }


        public GameBO? Find(Guid gameId)
        {
            return _games.FirstOrDefault(g => g.Id == gameId);
        }

        public List<GameBO> FindAll()
        {
            return _games;
        }

        public void Delete(int? userId)
        {
            GameBO? g = _games.FirstOrDefault(g => g.YellowPlayerId == userId || g.RedPlayerId == userId);
            if(g != null)
            {
                _games.Remove(g);
            }
        }

        public void Remove(GameBO game)
        {
            _games.Remove(game);
        }

        public GameBO Leave(Guid gameId, int? playerId)
        {
            GameBO? game = Find(gameId)
                ?? throw new GameException("This game does not exist");

            if (game.YellowPlayerId == playerId)
            {
                game.YellowPlayerStatus = PlayerStatus.Disconnected;
                if(game.Winner == null)
                {
                    game.Winner = P4Color.Red;
                    Remove(game);
                }
            }
            if (game.RedPlayerId == playerId)
            {
                game.RedPlayerStatus = PlayerStatus.Disconnected;
                if (game.Winner == null)
                {
                    game.Winner = P4Color.Yellow;
                    Remove(game);
                }
            }

            return game;
        }

        public GameBO Reconnect(Guid gameId, int? playerId)
        {
            GameBO? game = Find(gameId)
                ?? throw new GameException("This game does not exist");

            if (game.YellowPlayerId == playerId)
            {
                game.YellowPlayerStatus = PlayerStatus.Connected;
            }
            if (game.RedPlayerId == playerId)
            {
                game.RedPlayerStatus = PlayerStatus.Connected;
            }
            return game;
        }

        public GameBO Disconnect(Guid gameId, int? playerId)
        {
            GameBO? game = Find(gameId)
                ?? throw new GameException("This game does not exist");

            if (game.YellowPlayerId == playerId)
            {
                game.YellowPlayerStatus = PlayerStatus.Connecting;
            }
            if (game.RedPlayerId == playerId)
            {
                game.RedPlayerStatus = PlayerStatus.Connecting;
            }
            return game;
        }

        public GameBO? FindByPlayerId(int? playerId)
        {
            return _games.FirstOrDefault(g => g.RedPlayerId == playerId || g.YellowPlayerId == playerId);
        }

        public GameBO ClaimVictory(Guid gameId, int? playerId)
        {
            GameBO? game = Find(gameId)
                ?? throw new GameException("This game does not exist");

            if (game.YellowPlayerId == playerId && game.RedPlayerStatus == PlayerStatus.Connecting)
            {
                game.Winner = P4Color.Yellow;
                Remove(game);
            }
            if (game.RedPlayerId == playerId && game.YellowPlayerStatus == PlayerStatus.Connecting)
            {
                game.Winner = P4Color.Red;
                Remove(game);
            }

            return game;
        }
    }
}
