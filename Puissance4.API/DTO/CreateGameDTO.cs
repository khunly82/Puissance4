using Puissance4.Domain.Enums;

namespace Puissance4.API.DTO
{
    public class CreateGameDTO
    {
        public P4Color Color { get; set; }
        public bool VersusAI { get; set; }
    }
}
