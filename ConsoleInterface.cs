using MazeGen;

internal class ConsoleInterface
{
    private Maze? lastMaze = null;

    static readonly string[]?[] args = [
        null,
        ["Generator", "Width", "Height"],
        ["Walker"],
        null,
        null,
        null
    ];

    static int ParseCommand (string command) => command switch
    {
        "end" => 0,
        "gen" => 1,
        "walk" => 2,
        "print" => 3,
        "exp" => 4,
        "imp" => 5,
        _ => -1
    };

    private string? ExecuteCommand (int command, string[] args) => command switch
    {
        1 => (lastMaze = GenerateMaze(args[0], args[1], args[2]))?.AsciiMaze(),
        2 => lastMaze is null ? "No maze to walk." : WalkMaze(args[0], lastMaze),
        3 => lastMaze?.AsciiMaze() ?? "No maze to print",
        4 => lastMaze?.ToString() ?? "No maze to export",
        5 => "Importing not yet implemented.",
        _ => null
    };

    public void Run ()
    {
        Console.ResetColor();
        Console.Write("> ");

        Console.ForegroundColor = ConsoleColor.Yellow;
        string input = Console.ReadLine()!;

        if (input == "")
        {
            Console.CursorTop--;
            Run();
            return;
        }

        int command = ParseCommand(input);

        if (command == 0)
            return;

        if (command == -1)
        {
            Console.Write("Invalid command.\n\n");
            Run();
            return;
        }

        if (args[command] is null)
        {
            Console.ResetColor();
            Console.WriteLine(ExecuteCommand(command, []) ?? "Invalid inputs.");
            Console.WriteLine();
            Run();
            return;
        }

        string[] argInputs = new string[args[command]!.Length];

        Console.ForegroundColor = ConsoleColor.Cyan;
        foreach (string arg in args[command]!)
            Console.WriteLine(arg + ':');

        Console.CursorTop -= argInputs.Length;

        Console.ResetColor();

        for (int i = 0; i < argInputs.Length; i++)
        {
            Console.CursorLeft = args[command]![i].Length + 2;
            argInputs[i] = Console.ReadLine()!;
        }

        Console.WriteLine(ExecuteCommand(command, argInputs) ?? "Invalid inputs.");
        Console.WriteLine();
        Run();
    }

    private static Maze? GenerateMaze (string generator, string width, string height) => !int.TryParse(width, out int intWidth) || !int.TryParse(height, out int intHeight)
        ? null
        : generator switch
        {
            "rb" => RecursiveBacktracker.GenerateMaze(intWidth, intHeight),
            _ => null
        };

    private static string? WalkMaze (string walker, Maze maze)
    {
        byte[]? dirs = walker switch
        {
            "rb" => RecursiveBacktracker.WalkMaze(maze),
            _ => null
        };

        return dirs is null ? null : string.Join('\n', dirs.Select(DirName));
    }

    private static string DirName (byte dir) => dir switch
    {
        0b1000 => "up",
        0b0100 => "right",
        0b0010 => "down",
        0b0001 => "left",
        _ => null!
    };
}
