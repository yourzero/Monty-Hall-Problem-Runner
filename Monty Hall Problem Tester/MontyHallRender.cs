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
    private const string Magenta = "\x1b[31m"; // loser
    private const string Cyan    = "\x1b[36m"; // unknown

    private static string Colorize(string glyph, string ansi) => $"{ansi}{glyph}{Reset}";

    // ── Glyphs (assume monospace = all 1 column wide) ─────────────────────────
    private const string WinnerGlyph  = "⭐";
    private const string LoserGlyph   = "✖";
    private const string UnknownGlyph = "⁇";

    // ── Public: doors + status box side-by-side (width-aware) ─────────────────
    /// <summary>
    /// Renders 3 doors side-by-side. If there is room, prints the status box to the RIGHT;
    /// otherwise, falls back to printing the status box BELOW the doors to avoid terminal wrapping.
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
        int statusInnerWidth = lines.Max(l => l.Length);
        string top    = "┌" + new string('─', statusInnerWidth + 2) + "┐";
        string bottom = "└" + new string('─', statusInnerWidth + 2) + "┘";

        var statusBlock = new List<string>(lines.Count + 2) { top };
        foreach (var l in lines) statusBlock.Add("│ " + l.PadRight(statusInnerWidth) + " │");
        statusBlock.Add(bottom);

        int statusRows = statusBlock.Count;

        // ── width-aware layout decision ──
        int doorsWidth = doorRowStrings[0].Length;                  // total width of the stitched doors line
        int statusWidth = statusBlock[0].Length;                    // width of the status box
        int totalSideBySideWidth = initialPadding + doorsWidth + spaceBetweenDoorsAndStatus + statusWidth;

        // Use Console.WindowWidth if available; otherwise assume a wide window.
        int winWidth;
        try { winWidth = Math.Max(1, Console.WindowWidth); }
        catch { winWidth = int.MaxValue / 4; }

        bool fitsSideBySide = totalSideBySideWidth <= winWidth;

        var sb = new StringBuilder();

        if (fitsSideBySide)
        {
            // Combine doors (left) + gap + status (right)
            string gapRight = new string(' ', spaceBetweenDoorsAndStatus);
            int rows = Math.Max(doorRows, statusRows);

            for (int r = 0; r < rows; r++)
            {
                string doorPart   = (r < doorRows)   ? doorRowStrings[r] : new string(' ', doorsWidth);
                string statusPart = (r < statusRows) ? statusBlock[r]    : "";
                sb.Append(leftPad).Append(doorPart).Append(gapRight).Append(statusPart);
                if (r < rows - 1) sb.AppendLine();
            }
        }
        else
        {
            // Fallback: render doors first, then status box below with same left padding
            for (int r = 0; r < doorRows; r++)
            {
                sb.Append(leftPad).Append(doorRowStrings[r]);
                if (r < doorRows - 1) sb.AppendLine();
            }
            sb.AppendLine();
            for (int r = 0; r < statusRows; r++)
            {
                sb.Append(leftPad).Append(statusBlock[r]);
                if (r < statusRows - 1) sb.AppendLine();
            }
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

        // Colored center glyph (monospace → single cell)
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

            // Insert three cells "[space][glyph][space]" and start 1 cell LEFT of center
            int start = Math.Max(1, mid - 2) +1;
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
