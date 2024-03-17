namespace Puissance4.API.DTO
{
    public class MoveDTO
    {
        public MoveDTO(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
