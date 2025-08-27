namespace Monty_Hall_Problem_Tester;

using System;
using System.Linq;
using System.Text;

public static class MontyHallRender
{
    // Symbols (tweak to taste)
    private const char Shade               = '░';
    private const char WinnerSymbol        = '★';
    private const char LoserSymbol         = '×';
    private const char UnknownSymbol       = '?';

    // Public entry
    public static string RenderDoors(List<DoorStatus> doors, bool includeLabels = true)
    {
        if (doors is null) throw new ArgumentNullException(nameof(doors));
        if (doors.Count != 3) throw new ArgumentException("Exactly 3 doors are required.", nameof(doors));

        Console.OutputEncoding = Encoding.UTF8; // for box-drawing + shading

        // Render each door independently, then stitch side-by-side
        var blocks = doors.Select((d, i) => RenderOneDoor(d, i + 1, includeLabels)).ToArray();
        var lineCount = blocks[0].Length;

        var sb = new StringBuilder();
        for (int r = 0; r < lineCount; r++)
        {
            sb.Append(blocks[0][r]).Append("   ")
              .Append(blocks[1][r]).Append("   ")
              .Append(blocks[2][r]);
            if (r < lineCount - 1) sb.AppendLine();
        }
        return sb.ToString();
    }

    // Draws a single door as a 5x5 box + optional caption line under it
    private static string[] RenderOneDoor(DoorStatus door, int number, bool includeLabel, int initialPadding, int spaceBetweenDoors)
    {
        const int w = 5, h = 5;              // interior is 3x3
        var grid = new char[h][];
        grid[0] = "┌───┐".ToCharArray();
        for (int r = 1; r < h - 1; r++)
        {
            grid[r] = "│   │".ToCharArray();
        }
        grid[h - 1] = "└───┘".ToCharArray();

        // 1) Fill interior: shade if unopened, blank if opened
        var fill = door.DoorOpenState switch
        {
            DoorOpenState.Unopened => Shade,
            DoorOpenState.Opened   => ' ',
            _ => ' '
        };
        for (int r = 1; r < h - 1; r++)
            for (int c = 1; c < w - 1; c++)
                grid[r][c] = fill;

        // 2) Compute knowledge symbol (center cell [2,2]).
        //    Rules:
        //      - Unknown: only shown if unopened.
        //      - KnownWinner: always shown (even if opened).
        //      - KnownLoser: always shown (even if opened).
        char? knowledgeSymbol = door.DoorKnowledge switch
        {
            DoorKnowledge.Unknown     => (door.DoorOpenState == DoorOpenState.Unopened) ? UnknownSymbol : (char?)null,
            DoorKnowledge.KnownWinner => WinnerSymbol,
            DoorKnowledge.KnownLoser  => LoserSymbol,
            _ => null
        };
        if (knowledgeSymbol.HasValue)
            grid[2][2] = knowledgeSymbol.Value;

        // 3) Caption shows pick state. We avoid cluttering the door face.
        //    (n P) for player-picked, (n H) for host-picked, (n) otherwise.
        string caption = includeLabel
            ? door.DoorPickedState switch
            {
                DoorPickedState.PickedByPlayer => $" ({number} P)",
                DoorPickedState.PickedByHost   => $" ({number} H)",
                DoorPickedState.Unpicked       => $" ({number})",
                _ => $" ({number})"
            }
            : null;

        // Finalize lines
        var lines = grid.Select(row => new string(row)).ToList();
        if (includeLabel)
        {
            // Fit/pad to width 5
            if (caption!.Length > w) caption = caption.Substring(0, w);
            lines.Add(caption.PadLeft((w + caption.Length) / 2).PadRight(w));
        }

        return lines.ToArray();
    }

    public static bool RenderDoors(List<DoorStatus> getDoorStatusesInOrder, bool includeLabels, int initialPadding, int spaceBetweenDoors)
    {
        throw new NotImplementedException();
    }
}
