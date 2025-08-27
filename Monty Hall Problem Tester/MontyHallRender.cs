namespace Monty_Hall_Problem_Tester;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MontyHallRender
{
    // ── ANSI colors ───────────────────────────────────────────────────────────
    private const string Reset   = "\x1b[0m";
    private const string Yellow  = "\x1b[33m"; // winner
    private const string Magenta = "\x1b[35m"; // loser
    private const string Cyan    = "\x1b[36m"; // unknown

    private static string Colorize(string glyph, string ansi) => $"{ansi}{glyph}{Reset}";

    // ── Glyphs (assume monospace = all 1 column wide) ─────────────────────────
    private const string WinnerGlyph  = "⭐"; // large-looking but single cell in monospace
    private const string LoserGlyph   = "✖";
    private const string UnknownGlyph = "⁇";

    // ── Public: doors + status box side-by-side ───────────────────────────────
    /// <summary>
    /// Renders 3 doors side-by-side, with the status text box shown to the RIGHT of the doors.
    /// </summary>
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

        // Build the 3 door blocks (arrays of equal-height strings)
        var doorBlocks  = doors.Select((d, i) => RenderOneDoorColored(d, i + 1, includeLabels, doorWidth)).ToArray();
        int doorRows    = doorBlocks[0].Length;
        string between  = new string(' ', spaceBetweenDoors);
        string leftPad  = new string(' ', initialPadding);

        // Stitch each "row" of the three doors
        var doorRowStrings = new List<string>(doorRows);
        for (int r = 0; r < doorRows; r++)
            doorRowStrings.Add(doorBlocks[0][r] + between + doorBlocks[1][r] + between + doorBlocks[2][r]);

        // Build status box
        var lines = statusLines?.ToList() ?? new List<string> { "Current Status:" };
        int maxWidth = lines.Max(l => l.Length);
        string top    = "┌" + new string('─', maxWidth + 2) + "┐";
        string bottom = "└" + new string('─', maxWidth + 2) + "┘";

        var statusBlock = new List<string>(lines.Count + 2) { top };
        foreach (var l in lines) statusBlock.Add("│ " + l.PadRight(maxWidth) + " │");
        statusBlock.Add(bottom);

        int statusRows = statusBlock.Count;
        int rows       = Math.Max(doorRows, statusRows);
        string gapRight = new string(' ', spaceBetweenDoorsAndStatus);

        // Combine doors (left) + gap + status (right)
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

    // ── One door block (with number sign above and status letter below) ───────
    private static string[] RenderOneDoorColored(DoorStatus door, int number, bool includeLabel, int doorWidth)
    {
        int interiorWidth = doorWidth - 2;

        // Door body (5 rows: top/bottom border + 3 interior rows)
        char fillChar = door.DoorOpenState == DoorOpenState.Unopened ? '░' : ' ';
        var body = new List<string>(5)
        {
            "┌" + new string('─', interiorWidth) + "┐",
            "│" + new string(fillChar, interiorWidth) + "│",
            "│" + new string(fillChar, interiorWidth) + "│",
            "│" + new string(fillChar, interiorWidth) + "│",
            "└" + new string('─', interiorWidth) + "┘"
        };

        // Colored center glyph (assume monospace = single cell)
        string? glyph = door.DoorKnowledge switch
        {
            DoorKnowledge.KnownWinner => Colorize(WinnerGlyph,  Yellow),
            DoorKnowledge.KnownLoser  => Colorize(LoserGlyph,   Magenta),
            DoorKnowledge.Unknown     => (door.DoorOpenState == DoorOpenState.Unopened) ? Colorize(UnknownGlyph, Cyan) : null,
            _ => null
        };

        if (glyph != null)
        {
            // Visual center inside the borders
            int mid = 1 + (interiorWidth / 2);

            // Insert three cells "[space][glyph][space]" and purposely start 1 cell LEFT of center
            int start = Math.Max(1, mid - 2);
            body[2] = ReplaceNAt(body[2], start, 3, " " + glyph + " ");
        }

        // Top number sign (3 rows)
        string[] topLabel =
        {
            CenterToWidth("┌─┐", doorWidth),
            CenterToWidth($"│{number}│", doorWidth),
            CenterToWidth("└─┘", doorWidth),
        };

        // Bottom status marker (single letter centered)
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

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static string ReplaceNAt(string line, int start, int count, string replacement)
    {
        // Replaces exactly 'count' characters starting at 'start' with 'replacement'
        // (Assumes 'line' has no ANSI codes before replacement; we splice the colored glyph string here.)
        var before = line.Substring(0, start);
        var after  = (start + count <= line.Length) ? line.Substring(start + count) : "";
        return before + replacement + after;
    }

    private static string CenterToWidth(string s, int width)
    {
        if (s.Length >= width) return s.Length == width ? s : s.Substring(0, width);
        int left = (width - s.Length) / 2;
        return new string(' ', left) + s + new string(' ', width - left - s.Length);
    }
}
