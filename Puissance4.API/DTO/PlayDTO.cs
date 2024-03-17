using Puissance4.Domain.Enums;

namespace Puissance4.API.DTO
{
    public class PlayDTO
    {
        public int X { get; set; }
        public P4Color Color { get; set; }
        public Guid GameId { get; set; }
    }
}
