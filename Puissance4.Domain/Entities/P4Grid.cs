using Puissance4.Domain.Enums;
using System.Collections;

namespace Puissance4.Domain.Entities
{
    public class P4Grid: IEnumerable<P4Color>
    {

        public P4Grid()
        {
            
        }
        public P4Grid(P4Color[,] tiles)
        {
            Tiles = tiles;
            Count = this.Count(c => c != P4Color.None);
        }

        private P4Color[,] Tiles { get; set; } = new P4Color[7, 6];

        public int Count { get; set; }

        public P4Color this[int x, int y]
        {
            get
            {
                 return Tiles[x, y];
            }
            set
            {
                Tiles[x, y] = value;
            }
        }

        public P4Status? Status { get; set; } = null;

        public int Width => Tiles.GetLength(0);
        public int Height => Tiles.GetLength(1);

        public P4Grid Clone()
        {
            return new P4Grid((P4Color[,])Tiles.Clone());
        }

        public IEnumerator<P4Color> GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<P4Color> Iterate()
        {
            // form top -> bottom
            for (int i = Height - 1; i >= 0; i--)
            {
                // from left -> right
                for (int j = 0; j < Width; j++)
                {
                    yield return Tiles[j, i];
                }
            }
        }
    }
}
