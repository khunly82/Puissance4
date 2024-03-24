using Puissance4.Business.Exceptions;
using Puissance4.Business.Mappers;
using Puissance4.Domain.Enums;
using Puissance4.Business.BusinessObjects;

namespace Puissance4.Business.Services
{
    public class P4Service
    {
        public (int, int) Play(GridBO grid, int x, P4Color color)
        {
            if(grid.Status is not null)
            {
                throw new GameException("This game is finished");
            }

            if(!CheckTurn(grid, color))
            {
                throw new GameException("Not your turn");
            }

            int y = ColHeight(grid, x);
            if (y >= 6)
            {
                throw new GameException("The column is full");
            }

            grid.Count++;
            grid[x, y] = color;
            if (CheckVictory(grid, x, y, color))
            {
                grid.Status =
                    color == P4Color.Yellow
                    ? P4Status.YellowWin
                    : P4Status.RedWin;
            }
            else if (grid.Count >= 42)
            {
                grid.Status = P4Status.Draw;
            }
            return (x, y);
        }

        private bool CheckTurn(GridBO grid, P4Color color)
        {
            return (grid.Count % 2 == 0 && color == P4Color.Red)
                || (grid.Count % 2 == 1 && color == P4Color.Yellow);
        }

        public int ColHeight(GridBO grid, int x)
        {
            return Enumerable.Range(0, 6)
                .Select(i => grid[x, i])
                .Count(c => c != P4Color.None);
        }

        public bool CheckVictory(GridBO grid, int x, int y, P4Color color)
        {
            return CheckVertical(grid, x, y, color) 
                || CheckHorizontal(grid, x, y, color) 
                || CheckDiagonal(grid, x, y, color);
        }

        private bool CheckHorizontal(GridBO grid, int x, int y, P4Color color)
        {
            int c = 1;
            for (int dx = x + 1; dx < grid.Width; dx++)
            {
                if (grid[dx, y] != color)
                {
                    break;
                }
                c++;
            }
            for (int dx = x - 1; dx >= 0; dx--)
            {
                if (grid[dx, y] != color)
                {
                    break;
                }
                c++;
            }
            return c >= 4;
        }

        private bool CheckDiagonal(GridBO grid, int x, int y, P4Color color)
        {
            int c1 = 1;
            int c2 = 1;

            for (int dx = x + 1, dy = y + 1; dx < grid.Width && dy < grid.Height; dx++, dy++)
            {
                if (grid[dx, dy] != color)
                {
                    break;
                }
                c1++;
            }

            for (int dx = x - 1, dy = y - 1; dx >= 0 && dy >= 0; dx--, dy--)
            {
                if (grid[dx, dy] != color)
                {
                    break;
                }
                c1++;
            }

            if (c1 >= 4)
            {
                return true;
            }

            for (int dx = x + 1, dy = y - 1; dx < grid.Width && dy >= 0; dx++, dy--)
            {
                if (grid[dx, dy] != color)
                {
                    break;
                }
                c2++;
            }

            for (int dx = x - 1, dy = y + 1; dx >= 0 && dy < grid.Height; dx--, dy++)
            {
                if (grid[dx, dy] != color)
                {
                    break;
                }
                c2++;
            }

            if (c2 >= 4)
            {
                return true;
            }

            return false;
        }

        private bool CheckVertical(GridBO grid, int x, int y, P4Color color)
        {
            int c = 1;
            for (int dy = y - 1; dy >= 0; dy--)
            {
                if (grid[x, dy] != color)
                {
                    break;
                }
                c++;
            }
            return c >= 4;
        }

        public void Save(GridBO grid, string path)
        {
            if (grid.Status is null)
            {
                return;
            }
            using StreamWriter sw = File.AppendText(path);
            sw.WriteLine(string.Join(',', grid.Cast<int>().Append((int)grid.Status)));
        }

        public bool CanPlay(GridBO grid, int x)
        {
            return ColHeight(grid, x) < grid.Height;
        }

        public (int, int)? DirectPlay(GridBO grid, P4Color c)
        {
            // is winning move
            int? winningMove = GetWinningMove(grid, c);
            if(winningMove != null)
            {
                return Play(grid, (int)winningMove, c);
            }
            // is blocking move
            int? losingMove = GetWinningMove(grid, c.Switch());
            if (losingMove != null)
            {
                return Play(grid, (int)losingMove, c);
            }
            return null;
        }

        public int? GetWinningMove(GridBO grid, P4Color color)
        {
            for (int i = 0; i < grid.Width; i++)
            {
                if (CanPlay(grid, i))
                {
                    int y = ColHeight(grid, i);
                    if (CheckVictory(grid, i, y, color))
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        public (int, int) RandomPlay(GridBO grid, P4Color c)
        {
            try
            {
                return Play(grid, new Random().Next(0, grid.Width), c);
            }
            catch (Exception)
            {
                return RandomPlay(grid, c);
            }
        }

        public (int, int) AIPlay(GridBO grid, P4Color color, int depht)
        {

            try
            {
                
                (int?, float?) best = ComputeBest(grid, color, depht);

                if (best.Item1 is null)
                {
                    return RandomPlay(grid, color);
                }
                else
                {
                    return Play(grid, (int)best.Item1, color);
                }
            }
            catch (Exception)
            {
                return RandomPlay(grid, color);
            }
        }

        private (int?, float?) ComputeBest(GridBO grid, P4Color color, int depth = 1)
        {
            //if you want to stop if it's a winning move
            int? winningMove = GetWinningMove(grid, color);
            if (winningMove is not null)
            {
                return (winningMove, float.PositiveInfinity);
            }

            if (depth == 0)
            {
                return (null, 0);
            }

            (int?, float?) best = (null, null);
            for (int x = 0; x < 7; x++)
            {
                if (CanPlay(grid, x))
                {
                    int coeff = (int)color;
                    GridBO copy = grid.Clone();
                    copy[x, ColHeight(grid, x)] = color;
                    var input = copy.ToML();
                    var output = MLP4Model.Predict(input);
                    //Console.SetCursorPosition(0, x);
                    //Console.WriteLine($"{x}:{output.Score}");
                    float computed = (coeff * output.Score) - ComputeBest(copy, color.Switch(), depth - 1).Item2 ?? 0;
                    if (best.Item2 is null || computed > best.Item2)
                    {
                        best = (x, computed);
                    }
                }
            }
            return (best.Item1, best.Item2);
        }
    }
}
