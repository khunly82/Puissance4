using Puissance4.Business.BusinessObjects;
using Puissance4.Business.Services;
using Puissance4.Domain.Enums;
using ShellProgressBar;

P4Service service = new P4Service();

Train(1, true);
//Train(100, false);


void Train(int times, bool visible)
{
    int r = 0;
    int y = 0;
    int d = 0;

    ProgressBar? bar = null;
    if(!visible)
    {
        bar = new ProgressBar(times, "In Progress");
    }

    for (int i = 0; i < times; i++)
    {
        GridBO grid = new GridBO();
        P4Color color = P4Color.Yellow;

        while (grid.Status == null)
        {
            color = color.Switch();
            //if (i % 2 == 0)
            //if(color == P4Color.Yellow)
            if(color == P4Color.Red)
            {
                service.AIPlay(grid, color, 3);
            }
            else
            {
                service.AIPlay(grid, color, 3);
                //var _ = service.DirectPlay(grid, color) ?? service.RandomPlay(grid, color);
            }
            if (visible)
            {
                Display(grid);
                Thread.Sleep(500);
            }
        }
        if(visible)
        {
            //Console.WriteLine(grid.Status);
        }
        switch (grid.Status)
        {
            case P4Status.YellowWin:
                y++;
                break;
            case P4Status.RedWin: 
                r++; break;
            default:
                d++;
                break;
        }
        //service.Save(grid, "C:\\Users\\K\\Desktop\\Puissance4\\Puissance4.Business\\Data\\games.csv");
        bar?.Tick();
    }
    bar?.Dispose();
    Console.SetCursorPosition(0, 5);
    Console.WriteLine($"R: {r}\nY: {y}\nD: {d}");
}

void Display(GridBO grid)
{
    Console.Clear();

    for (int x = 0; x < grid.Width; x++)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            Console.SetCursorPosition(x, 6 - y + 15);
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
    Console.Write(v == P4Color.None ? "" : "0");
    Console.ResetColor();
}


Console.ReadKey();