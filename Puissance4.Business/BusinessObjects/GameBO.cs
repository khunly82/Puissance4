using Puissance4.Business.Enums;
using Puissance4.Domain.Enums;

namespace Puissance4.Business.BusinessObjects
{
    public class GameBO
    {
        public Guid Id { get; set; }
        public string Name { get => Id.ToString(); }
        public int? RedPlayerId { get; set; }
        public int? YellowPlayerId { get; set; }
        public string? RedPlayerName { get; set; }
        public string? YellowPlayerName { get; set; }
        public PlayerStatus? RedPlayerStatus { get; set; }
        public PlayerStatus? YellowPlayerStatus { get; set; }
        public bool VersusAI { get; set; }
        public P4Color? Winner { get => (P4Color?)(int?)Grid.Status; set => Grid.Status = (P4Status?)(int?)value; }
        public GridBO Grid { get; set; } = new GridBO();

    }
}
