using Puissance4.Business.BusinessObjects;
using Puissance4.Domain.Enums;

namespace Puissance4.API.DTO
{
    public class GameByPlayerDTO
    {
        public GameByPlayerDTO(GameBO g)
        {
            Id = g.Id;
            RedPlayerId = g.RedPlayerId;
            YellowPlayerId = g.YellowPlayerId;
            RedPlayerName = g.RedPlayerName;
            YellowPlayerName = g.YellowPlayerName;
            VersusAI = g.VersusAI;
            AIDepth = g.AIDepth;
            Winner = g.Winner;
            Grid = new P4Color[7][];
            for (int x = 0; x < g.Grid.Width; x++)
            {
                Grid[x] = new P4Color[6];
                for (int y = 0; y < g.Grid.Height; y++)
                {
                    Grid[x][y] = g.Grid[x, y];
                }
            }
        }

        public Guid Id { get; set; }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public string? RedPlayerName { get; set; }
        public string? YellowPlayerName { get; set; }
        public bool VersusAI { get; set; }
        public int? AIDepth { get; set; }
        public P4Color? Winner { get; set; }
        public P4Color[][]? Grid { get; set; }
    }
}
