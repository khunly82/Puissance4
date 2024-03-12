using Puissance4.Business.Enums;

namespace Puissance4.Business
{
    public class P4Grid
    {
        public P4Color?[,] Tiles { get; private set; } = new P4Color?[7,6];

        public P4Color? this[int x,int y]
        {
            get
            {
                try
                {
                    return Tiles[x,y];
                } catch
                {
                    return null;
                }
            }
        }

        public P4Status? Status { get; private set; } = null;

        public int Play(int x, P4Color color)
        {
            int y = ColHeight(x);
            if(y >= 6)
            {
                throw new Exception();
            }
            Tiles[x,y] = color;
            if(CheckVictory(x, y, color))
            {
                Status = 
                    color == P4Color.Yellow 
                    ? P4Status.YellowWin 
                    : P4Status.RedWin;
                return y;
            }
            if(Tiles.Cast<P4Color?>().All(t => t != null))
            {
                Status = P4Status.Draw;
                return y;
            }
            return y;
        }

        private int ColHeight(int x)
        {
            return Enumerable.Range(0, 6)
                .Select(i => Tiles[x, i])
                .Count(c => c != null);
        }

        private bool CheckVictory(int x, int y, P4Color color)
        {
            return CheckVertical(x, y, color) || CheckHorizontal(x, y, color) || CheckDiagonal(x, y, color);
        }

        private bool CheckHorizontal(int x, int y, P4Color color)
        {
            return 
                Count((x, y) => (x - 1, y), x, y, color) 
                + 1 
                + Count((x, y) => (x + 1, y), x, y, color) 
                >= 4;
        }

        private bool CheckDiagonal(int x, int y, P4Color color)
        {
            return
                Count((x, y) => (x - 1, y - 1), x, y, color)
                + 1
                + Count((x, y) => (x + 1, y + 1), x, y, color)
                >= 4
                ||
                Count((x, y) => (x + 1, y - 1), x, y, color)
                + 1
                + Count((x, y) => (x - 1, y + 1), x, y, color)
                >= 4
                ;
        }

        private bool CheckVertical(int x, int y, P4Color color)
        {
            return 1 + Count((x, y) => (x, y - 1), x, y, color) >= 4;
        }

        private int Count(Func<int, int, (int, int)> direction, int fromX, int fromY, P4Color color)
        {
            (int x, int y) = direction(fromX, fromY);
            if (this[x, y] != color)
            {
                return 0;
            }
            return 1 + Count(direction, x, y, color);
        }

    }
}
