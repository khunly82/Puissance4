using Puissance4.Domain.Enums;

namespace Puissance4.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; set; }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public string SerializedGrid { get; set; } = null!;
        public P4Color Winner { get; set; }
        public bool VersusAI { get; set; }
    }
}
