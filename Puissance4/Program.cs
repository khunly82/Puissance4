using Puissance4.Business;
using Puissance4.Business.Enums;

P4Grid grid = new P4Grid();
P4Color color = P4Color.Yellow;

while (grid.Status == null)
{
    //Thread.Sleep(500);
    color = (P4Color)(((int)color + 1) % 2);
    Play(color);
    Display(grid);
}

void Play(P4Color c)
{
    try
    {
        grid.Play(new Random().Next(0, 7), c);
    }
    catch (Exception)
    {

        Play(c);
    }
}


void Display(P4Grid grid)
{
    Console.Clear();
    for (int x = 0; x < grid.Tiles.GetLength(0); x++)
    {
        for (int y = 0; y < grid.Tiles.GetLength(1); y++)
        {
            Console.SetCursorPosition(x, 6 - y);
            P4Color? color = grid[x, y];
            if(color is P4Color c)
            {
                DisplayTile(c);
            }
        }
    }
}

void DisplayTile(P4Color v)
{
    Console.ForegroundColor = 
        v == P4Color.Red 
        ? ConsoleColor.Red 
        : ConsoleColor.Yellow; 
    Console.Write("0");
    Console.ResetColor();
}

Console.SetCursorPosition(0, 10);
Console.WriteLine(grid.Status);
Console.ReadKey();