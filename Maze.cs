namespace MazeGen;
using System.Text;

public record Maze (int Width, int Height)
{
    private readonly byte[,] grid = new byte[Width, Height];

    public int Size => Width * Height;

    public byte this[int x, int y] { get => grid[x, y]; set => grid[x, y] = value; }

    public byte this[(int x, int y) cell] { get => grid[cell.x, cell.y]; set => grid[cell.x, cell.y] = value; }

    public override string ToString ()
    {
        StringBuilder builder = new($"{Width:D3}{Height:D3}");

        foreach (byte cell in grid)
            _ = builder.Append(cell);

        return builder.ToString();
    }

    public string AsciiMaze () => string.Concat(AsciiCharArray());

    private IEnumerable<char> AsciiCharArray ()
    {
        yield return ' ';

        for (int i = 1; i < (Width * 2); i++)
            yield return '_';

        for (int row = 0; row < Height; row++)
        {
            yield return '\n';

            for (int col = 0; col < Width; col++)
            {
                yield return (grid[col, row] & 0b01) == 0 ? '|' : row != Height - 1 && (grid[col, row + 1] & 0b01) == 0 ? '.' : '_';
                yield return row == Height - 1 || (grid[col, row + 1] & 0b10) == 0 ? '_' : ' ';
            }

            yield return '|';
        }
    }
}
