namespace MazeGen;

using System.Collections.Generic;

public class RecursiveBacktracker : IMazeGenerator, IMazeWalker
{
    private RecursiveBacktracker () { }

    public static Maze GenerateMaze (int width, int height)
    {
        Maze maze = new(width, height);
        bool[,] visited = new bool[width, height];

        CarvePassages((Random.Shared.Next(maze.Width - 1), Random.Shared.Next(maze.Height - 1)), ref maze, ref visited);

        return maze;
    }

    private static void CarvePassages ((int x, int y) cell, ref Maze maze, ref bool[,] visited)
    {
        visited[cell.x, cell.y] = true;

        byte[] dirs = [0b1000, 0b0100, 0b0010, 0b0001];
        Random.Shared.Shuffle(dirs);

        foreach (byte dir in dirs)
        {
            switch (dir)
            {
                case 0b1000:
                    if (cell.y == 0 || visited[cell.x, cell.y - 1])
                        continue;

                    maze[cell] |= 0b10;
                    CarvePassages((cell.x, cell.y - 1), ref maze, ref visited);
                    break;

                case 0b0100:
                    if (cell.x == maze.Width - 1 || visited[cell.x + 1, cell.y])
                        continue;

                    maze[cell.x + 1, cell.y] |= 0b01;
                    CarvePassages((cell.x + 1, cell.y), ref maze, ref visited);
                    break;

                case 0b0010:
                    if (cell.y == maze.Height - 1 || visited[cell.x, cell.y + 1])
                        continue;

                    maze[cell.x, cell.y + 1] |= 0b10;
                    CarvePassages((cell.x, cell.y + 1), ref maze, ref visited);
                    break;

                case 0b0001:
                    if (cell.x == 0 || visited[cell.x - 1, cell.y])
                        continue;

                    maze[cell] |= 0b01;
                    CarvePassages((cell.x - 1, cell.y), ref maze, ref visited);
                    break;
            }
        }
    }

    public static byte[] WalkMaze (Maze maze)
    {
        List<byte> path = [];

        _ = WalkMaze((0, 0), -1, (maze.Width - 1, maze.Height - 1), maze, ref path);
        path.Reverse();

        return path.ToArray();
    }

    private static bool WalkMaze ((int x, int y) cell, int invalidDir, (int x, int y) targetCell, Maze maze, ref List<byte> path)
    {
        byte[] dirs = [0b1000, 0b0100, 0b0010, 0b0001];
        
        if (invalidDir != -1)
            dirs[invalidDir] = 0;

        (int x, int y) nextCell = (-1, -1);
        int reverseDir = -1;
        
        foreach (byte dir in dirs)
        {
            switch (dir)
            {
                case 0:
                    continue;

                case 0b1000:
                    if ((maze[cell] & 0b10) == 0)
                        continue;

                    reverseDir = 2;
                    nextCell = (cell.x, cell.y - 1);
                    break;

                case 0b0100:
                    if (cell.x == maze.Width - 1 || (maze[cell.x + 1, cell.y] & 0b01) == 0)
                        continue;

                    reverseDir = 3;
                    nextCell = (cell.x + 1, cell.y);
                    break;

                case 0b0010:
                    if (cell.y == maze.Height - 1 || (maze[cell.x, cell.y + 1] & 0b10) == 0)
                        continue;

                    reverseDir = 0;
                    nextCell = (cell.x, cell.y + 1);
                    break;

                case 0b0001:
                    if (cell.x == 0 || (maze[cell] & 0b01) == 0)
                        continue;

                    reverseDir = 1;
                    nextCell = (cell.x - 1, cell.y);
                    break;
            }
            
            if (nextCell == targetCell || WalkMaze(nextCell, reverseDir, targetCell, maze, ref path))
            {
                path.Add(dir);
                return true;
            }
        }

        return false;
    }
}
