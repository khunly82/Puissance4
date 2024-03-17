using Puissance4.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puissance4.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; set; }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public Player? RedPlayer { get; set; } = null!;
        public Player? YellowPlayer { get; set; } = null!;
        public string SerializedGrid { get; set; } = null!;
        public P4Color? Winner { get; set; }
        public bool VersusAI { get; set; }

        [NotMapped]
        public P4Grid Grid { get; set; } = new P4Grid();
    }
}
