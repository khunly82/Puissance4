using Puissance4.Domain.Entities;
using Puissance4.Domain.Enums;

namespace Puissance4.API.DTO
{
    public class GameDTO
    {

        public GameDTO(Game g, bool full = false)
        {
            Id = g.Id;
            RedPlayerId = g.RedPlayerId;
            YellowPlayerId = g.YellowPlayerId;
            RedPlayerName = g.RedPlayer?.Username;
            YellowPlayerName = g.YellowPlayer?.Username;
            if(full)
            {
                Grid = new P4Color[7][];
                for(int x = 0; x < g.Grid.Width; x++)
                {
                    Grid[x] = new P4Color[6];
                    for(int y = 0; y < g.Grid.Height; y++)
                    {
                        Grid[x][y] = g.Grid[x, y];
                    }
                }
            }
        }

        public Guid Id { get; set; }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public string? RedPlayerName { get; set; }
        public string? YellowPlayerName { get; set; }

        public P4Color[][]? Grid{ get; set; }
    }
}
