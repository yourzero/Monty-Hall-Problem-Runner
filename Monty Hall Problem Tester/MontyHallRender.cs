namespace Monty_Hall_Problem_Tester;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MontyHallRender
{
    // Single-width symbols (avoid emoji double-width)
    private const string WinnerGlyph  = "⭐";
    private const string LoserGlyph   = "✖";
    private const string UnknownGlyph = "⁇";

    // ANSI colors (customize)
    private const string Reset   = "\x1b[0m";
    private const string Yellow  = "\x1b[33m"; // winner
    private const string Magenta = "\x1b[35m"; // loser
    private const string Cyan    = "\x1b[36m"; // unknown

    private static string Color(string glyph, string ansi) => $"{ansi}{glyph}{Reset}";

    /// <summary>
    /// Renders exactly 3 doors side-by-side with colored center glyphs.
    /// </summary>
    public static string RenderDoors(
        List<DoorStatus> doors,
        bool includeLabels,
        int initialPadding,
        int spaceBetweenDoors,
        int doorWidth)
    {
        if (doors is null) throw new ArgumentNullException(nameof(doors));
        if (doors.Count != 3) throw new ArgumentException("Exactly 3 doors are required.", nameof(doors));
        if (doorWidth < 5) throw new ArgumentException("Door width must be at least 5.", nameof(doorWidth));

        Console.OutputEncoding = Encoding.UTF8;

        var blocks = doors.Select((d, i) => RenderOneDoorColored(d, i + 1, includeLabels, doorWidth)).ToArray();
        var lineCount = blocks[0].Length;

        string leftPad = new string(' ', initialPadding);
        string midPad  = new string(' ', spaceBetweenDoors);

        var sb = new StringBuilder();
        for (int r = 0; r < lineCount; r++)
        {
            sb.Append(leftPad)
              .Append(blocks[0][r]).Append(midPad)
              .Append(blocks[1][r]).Append(midPad)
              .Append(blocks[2][r]);
            if (r < lineCount - 1) sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    /// Single door with number sign on top and status letter below.
    /// Center symbol is colored via ANSI (keeps single-cell width).
    /// </summary>
    private static string[] RenderOneDoorColored(DoorStatus door, int number, bool includeLabel, int doorWidth)
    {
        int interiorWidth = doorWidth - 2;

        // Build door body as strings (so we can splice colored glyphs)
        var body = new List<string>(5)
        {
            "┌" + new string('─', interiorWidth) + "┐",
            "│" + new string(door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ', interiorWidth) + "│",
            "│" + new string(door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ', interiorWidth) + "│",
            "│" + new string(door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ', interiorWidth) + "│",
            "└" + new string('─', interiorWidth) + "┘"
        };

        // Choose colored center glyph
        string? glyph = door.DoorKnowledge switch
        {
            DoorKnowledge.KnownWinner => Color(WinnerGlyph,  Yellow),
            DoorKnowledge.KnownLoser  => Color(LoserGlyph,   Magenta),
            DoorKnowledge.Unknown     => (door.DoorOpenState == DoorOpenState.Unopened) ? Color(UnknownGlyph, Cyan) : null,
            _ => null
        };

        if (glyph != null)
        {
            // Center column inside the bordered line (absolute column in the full string)
            int centerColAbs = 1 + (interiorWidth / 2); // border at 0, interior starts at 1
            body[2] = ReplaceAt(body[2], centerColAbs, glyph); // middle row of the 3 interior rows
            //body[2] = ReplaceAt(body[2], centerColAbs, glyph); // middle row of the 3 interior rows
        }

        // Top number sign (3x1)
        string[] topLabel =
        {
            CenterToWidth("┌─┐", doorWidth),
            CenterToWidth($"│{number}│", doorWidth),
            CenterToWidth("└─┘", doorWidth),
        };

        // Bottom status marker (single letter)
        char statusMarker = door.DoorPickedState switch
        {
            DoorPickedState.PickedByPlayer => 'P',
            DoorPickedState.PickedByHost   => 'H',
            _                              => '·'
        };
        string bottomLabel = CenterToWidth(statusMarker.ToString(), doorWidth);

        // Assemble full block
        var lines = new List<string>(topLabel);
        lines.AddRange(body);
        if (includeLabel) lines.Add(bottomLabel);
        return lines.ToArray();
    }

    // Replace the character at visual column 'index' with 'replacement' (a string, e.g., colored glyph).
    private static string ReplaceAt(string line, int index, string replacement)
    {
        // Assumes 'line' currently contains no ANSI sequences; index is a raw char index.
        return line.Substring(0, index) + replacement + line.Substring(index + 1);
    }

    // Centers a short string within a fixed width by left/right padding.
    private static string CenterToWidth(string s, int width)
    {
        if (s.Length >= width) return s.Length == width ? s : s.Substring(0, width);
        int left = (width - s.Length) / 2;
        return new string(' ', left) + s + new string(' ', width - left - s.Length);
    }
}
