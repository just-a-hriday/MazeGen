namespace MazeGen;

internal interface IMazeGenerator
{
    public static abstract Maze GenerateMaze (int mazeWidth, int mazeHeight);
}
