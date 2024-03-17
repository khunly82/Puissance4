using Puissance4.Domain.Entities;

namespace Puissance4.API.DTO
{
    public class GameDTO
    {

        public GameDTO(Game g)
        {
            Id = g.Id;
            RedPlayerId = g.RedPlayerId;
            YellowPlayerId = g.YellowPlayerId;
            RedPlayerName = g.RedPlayer?.Username;
            YellowPlayerName = g.YellowPlayer?.Username;
        }

        public Guid Id { get; set; }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public string? RedPlayerName { get; set; }
        public string? YellowPlayerName { get; set; }
    }
}
