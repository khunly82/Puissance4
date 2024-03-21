using Puissance4.DAL.Repositories;
using Puissance4.Domain.Entities;

namespace Puissance4.Business.Services
{
    public class PlayerService(PlayerRepository playerRepository)
    {
        public void Register(string username, string password)
        {
            Player? player = playerRepository.FindByUsername(username);
            if (player != null)
            {
                throw new Exception("This name is already used");
            }
            playerRepository.Add(new Player
            {
                Username = username,
                Password = password
            });
        }

        public Player Login(string username, string password)
        {
            Player? player = playerRepository.FindByUsername(username);
            if (player == null || player.Password != password)
            {
                throw new Exception("Invalid Credentials");
            }
            return player;
        }
    }
}
