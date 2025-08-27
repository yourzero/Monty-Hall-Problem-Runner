namespace Monty_Hall_Problem_Tester;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MontyHallRender
{
    // Keep your current glyphs/colors
    private const string WinnerGlyph  = "⭐";
    private const string LoserGlyph   = "✖";
    private const string UnknownGlyph = "⁇";

    private const string Reset   = "\x1b[0m";
    private const string Yellow  = "\x1b[33m"; // winner
    private const string Magenta = "\x1b[35m"; // loser
    private const string Cyan    = "\x1b[36m"; // unknown

    private static string Color(string glyph, string ansi) => $"{ansi}{glyph}{Reset}";

    // NEW: doors + status box side-by-side
    public static string RenderDoorsWithStatus(
        List<DoorStatus> doors,
        IEnumerable<string> statusLines,
        bool includeLabels,
        int initialPadding,
        int spaceBetweenDoors,
        int doorWidth,
        int spaceBetweenDoorsAndStatus)
    {
        if (doors is null) throw new ArgumentNullException(nameof(doors));
        if (doors.Count != 3) throw new ArgumentException("Exactly 3 doors are required.", nameof(doors));
        if (doorWidth < 5) throw new ArgumentException("Door width must be at least 5.", nameof(doorWidth));

        Console.OutputEncoding = Encoding.UTF8;

        // Build the 3 door blocks (each is an array of lines of fixed doorWidth)
        var doorBlocks  = doors.Select((d, i) => RenderOneDoorColored(d, i + 1, includeLabels, doorWidth)).ToArray();
        int doorRows    = doorBlocks[0].Length;
        string between  = new string(' ', spaceBetweenDoors);
        string leftPad  = new string(' ', initialPadding);

        // Prepare stitched door rows (single string per row)
        var doorRowStrings = new List<string>(doorRows);
        for (int r = 0; r < doorRows; r++)
        {
            doorRowStrings.Add(
                doorBlocks[0][r] + between + doorBlocks[1][r] + between + doorBlocks[2][r]
            );
        }

        // Build the status box (independently sized)
        var lines = statusLines?.ToList() ?? new List<string>();
        if (lines.Count == 0) lines.Add("Current Status:");

        int maxWidth = lines.Max(l => l.Length);
        string top    = "┌" + new string('─', maxWidth + 2) + "┐";
        string bottom = "└" + new string('─', maxWidth + 2) + "┘";

        var statusBlock = new List<string>(lines.Count + 2);
        statusBlock.Add(top);
        foreach (var l in lines)
            statusBlock.Add("│ " + l.PadRight(maxWidth) + " │");
        statusBlock.Add(bottom);

        int statusRows = statusBlock.Count;
        int rows = Math.Max(doorRows, statusRows);

        // Combine doors + gap + status (right)
        string gapRight = new string(' ', spaceBetweenDoorsAndStatus);
        var sb = new StringBuilder();
        for (int r = 0; r < rows; r++)
        {
            string doorPart   = (r < doorRows)   ? doorRowStrings[r] : new string(' ', doorRowStrings[0].Length);
            string statusPart = (r < statusRows) ? statusBlock[r]    : "";
            sb.Append(leftPad).Append(doorPart).Append(gapRight).Append(statusPart);
            if (r < rows - 1) sb.AppendLine();
        }
        return sb.ToString();
    }

    // Single door with number on top and status letter at bottom.
    private static string[] RenderOneDoorColored(DoorStatus door, int number, bool includeLabel, int doorWidth)
    {
        int interiorWidth = doorWidth - 2;

        var body = new List<string>(5)
        {
            "┌" + new string('─', interiorWidth) + "┐",
            "│" + new string(door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ', interiorWidth) + "│",
            "│" + new string(door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ', interiorWidth) + "│",
            "│" + new string(door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ', interiorWidth) + "│",
            "└" + new string('─', interiorWidth) + "┘"
        };

        // Colored center glyph
        string? glyph = door.DoorKnowledge switch
        {
            DoorKnowledge.KnownWinner => Color(WinnerGlyph,  Yellow),
            DoorKnowledge.KnownLoser  => Color(LoserGlyph,   Magenta),
            DoorKnowledge.Unknown     => (door.DoorOpenState == DoorOpenState.Unopened) ? Color(UnknownGlyph, Cyan) : null,
            _ => null
        };

        if (glyph != null)
        {
            // Visual center inside border
            int mid = 1 + (interiorWidth / 2);

            // Reserve [space][glyph][space], shifted one cell LEFT vs center
            bool canPadBothSides = (interiorWidth >= 5); // doorWidth >= 7
            if (canPadBothSides)
            {
                int start = Math.Max(1, mid - 2);                        // << shifted 1 cell more left
                if (start + 2 > interiorWidth) start = interiorWidth - 2;
                body[2] = Replace3At(body[2], start, glyph);             // [space][glyph][space]
            }
            else
            {
                // narrow door fallback
                body[2] = ReplaceAt(body[2], mid, glyph);
            }
        }

        // Top number sign (3 rows)
        string[] topLabel =
        {
            CenterToWidth("┌─┐", doorWidth),
            CenterToWidth($"│{number}│", doorWidth),
            CenterToWidth("└─┘", doorWidth),
        };

        // Bottom status marker
        char statusMarker = door.DoorPickedState switch
        {
            DoorPickedState.PickedByPlayer => 'P',
            DoorPickedState.PickedByHost   => 'H',
            _                              => '·'
        };
        string bottomLabel = CenterToWidth(statusMarker.ToString(), doorWidth);

        var lines = new List<string>(topLabel);
        lines.AddRange(body);
        if (includeLabel) lines.Add(bottomLabel);
        return lines.ToArray();
    }

    // Replace [start..start+2] with " glyph " (three cells)
    private static string Replace3At(string line, int start, string glyph)
    {
        var before = line.Substring(0, start);
        var after  = (start + 3 <= line.Length) ? line.Substring(start + 3) : "";
        return before + " " + glyph + " " + after;
    }

    private static string ReplaceAt(string line, int index, string glyph)
    {
        var before = line.Substring(0, index);
        var after  = (index + 1 <= line.Length) ? line.Substring(index + 1) : "";
        return before + glyph + after;
    }

    private static string CenterToWidth(string s, int width)
    {
        if (s.Length >= width) return s.Length == width ? s : s.Substring(0, width);
        int left = (width - s.Length) / 2;
        return new string(' ', left) + s + new string(' ', width - left - s.Length);
    }
}
