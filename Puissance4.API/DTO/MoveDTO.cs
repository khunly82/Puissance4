using Puissance4.Domain.Enums;

namespace Puissance4.API.DTO
{
    public class MoveDTO(int x, int y, P4Color color)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public P4Color Color { get; set; } = color;
    }
}
