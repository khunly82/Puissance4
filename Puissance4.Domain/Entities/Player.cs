namespace Puissance4.Domain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;

        public ICollection<Game> GamesAsRed { get; set; } = null!;
        public ICollection<Game> GamesAsYellow { get; set; } = null!;
    }
}
