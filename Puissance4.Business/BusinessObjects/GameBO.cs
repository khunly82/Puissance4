using Puissance4.Domain.Entities;
using Puissance4.Domain.Enums;

namespace Puissance4.Business.BusinessObjects
{
    public class GameBO
    {
        public Guid Id { get; set; }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public string? RedPlayerName { get; set; }
        public string? YellowPlayerName { get; set; }
        public bool? RedPlayerConnected { get; set; }
        public bool? YellowPlayerConnected { get; set; }
        public bool VersusAI { get; set; }
        public P4Color? Winner { get => (P4Color?)(int?)Grid.Status; }
        public P4GridBO Grid { get; set; } = new P4GridBO();

    }
}
