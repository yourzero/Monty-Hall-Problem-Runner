// namespace Monty_Hall_Problem_Tester;
//
// using System;
// using System.Linq;
// using System.Text;
// using System.Collections.Generic;
//
// public static class MontyHallRender
// {
//     // Symbols (tweak to taste)
//     // private const char Shade        = '░';
//     // private const char WinnerSymbol = '⭐';
//     // private const char LoserSymbol  = '❌';
//     // private const char UnknownSymbol= '❓';
//     //
//     //
//     // Symbols (single column glyphs)
//     private const char Shade         = '░';
//     private const char WinnerSymbol  = '⭐'; // or '★'
//     private const char LoserSymbol   = '✖';
//     private const char UnknownSymbol = '⁇';
//
// // Optional: ANSI coloring without changing width
//     private static string Color(string s, string ansi) => $"{ansi}{s}\x1b[0m";
//     private const string Yellow = "\x1b[33m";
//     private const string Magenta = "\x1b[35m";
//     
//     // Public entry (with padding args)
//     public static string RenderDoors(
//         List<DoorStatus> doors,
//         bool includeLabels,
//         int initialPadding,
//         int spaceBetweenDoors,
//         int doorWidth)
//     {
//         if (doors is null) throw new ArgumentNullException(nameof(doors));
//         if (doors.Count != 3) throw new ArgumentException("Exactly 3 doors are required.", nameof(doors));
//
//         Console.OutputEncoding = Encoding.UTF8; // for box-drawing + shading
//
//         // Render each door independently
//         var blocks = doors.Select((d, i) => RenderOneDoor(d, i + 1, includeLabels, doorWidth)).ToArray();
//         var lineCount = blocks[0].Length;
//
//         // Build spacing strings
//         string leftPad = new string(' ', initialPadding);
//         string midPad  = new string(' ', spaceBetweenDoors);
//
//         var sb = new StringBuilder();
//         for (int r = 0; r < lineCount; r++)
//         {
//             sb.Append(leftPad)
//               .Append(blocks[0][r]).Append(midPad)
//               .Append(blocks[1][r]).Append(midPad)
//               .Append(blocks[2][r]);
//
//             if (r < lineCount - 1) sb.AppendLine();
//         }
//
//         return sb.ToString();
//     }
//
//     // Draws a single door as a 5x5 box + optional caption line under it
//     private static string[] xRenderOneDoor(DoorStatus door, int number, bool includeLabel)
//     {
//         const int w = 5, h = 5;              // interior is 3x3
//         var grid = new char[h][];
//         grid[0] = "┌───┐".ToCharArray();
//         for (int r = 1; r < h - 1; r++)
//         {
//             grid[r] = "│   │".ToCharArray();
//         }
//         grid[h - 1] = "└───┘".ToCharArray();
//
//         // 1) Fill interior: shade if unopened, blank if opened
//         var fill = door.DoorOpenState switch
//         {
//             DoorOpenState.Unopened => Shade,
//             DoorOpenState.Opened   => ' ',
//             _ => ' '
//         };
//         for (int r = 1; r < h - 1; r++)
//         for (int c = 1; c < w - 1; c++)
//             grid[r][c] = fill;
//
//         // 2) Place knowledge symbol (center)
//         char? knowledgeSymbol = door.DoorKnowledge switch
//         {
//             DoorKnowledge.Unknown     => (door.DoorOpenState == DoorOpenState.Unopened) ? UnknownSymbol : (char?)null,
//             DoorKnowledge.KnownWinner => WinnerSymbol,
//             DoorKnowledge.KnownLoser  => LoserSymbol,
//             _ => null
//         };
//         if (knowledgeSymbol.HasValue)
//             grid[2][2] = knowledgeSymbol.Value;
//
//         // 3) Top label (door number inside a box)
//         string[] topLabel =
//         {
//             "┌───┐",
//             $"│ {number} │",
//             "└───┘"
//         };
//
//         // 4) Bottom label (status marker)
//         char statusMarker = door.DoorPickedState switch
//         {
//             DoorPickedState.PickedByPlayer => 'P',
//             DoorPickedState.PickedByHost   => 'H',
//             _                              => '·'
//         };
//
//         string bottomLabel = $"  {statusMarker}  "; // centered in 5 chars
//
//         // Build full block
//         var lines = new List<string>();
//         lines.AddRange(topLabel.Select(l => l.PadLeft((w + l.Length) / 2).PadRight(w))); // center top label
//         lines.AddRange(grid.Select(row => new string(row)));
//         if (includeLabel)
//             lines.Add(bottomLabel);
//
//         return lines.ToArray();
//     }
//     
//     private static string[] RenderOneDoor(DoorStatus door, int number, bool includeLabel, int doorWidth)
// {
//     if (doorWidth < 5) throw new ArgumentException("Door width must be at least 5.", nameof(doorWidth));
//
//     int interiorWidth = doorWidth - 2; // subtract borders
//     int h = 5;
//
//     // Build the door outline
//     var grid = new char[h][];
//     grid[0] = ("┌" + new string('─', interiorWidth) + "┐").ToCharArray();
//     for (int r = 1; r < h - 1; r++)
//     {
//         grid[r] = ("│" + new string(' ', interiorWidth) + "│").ToCharArray();
//     }
//     grid[h - 1] = ("└" + new string('─', interiorWidth) + "┘").ToCharArray();
//
//     // Fill interior shading if unopened
//     var fill = door.DoorOpenState == DoorOpenState.Unopened ? Shade : ' ';
//     for (int r = 1; r < h - 1; r++)
//         for (int c = 1; c <= interiorWidth; c++)
//             grid[r][c] = fill;
//
//     // Place knowledge symbol in center
//     char? knowledgeSymbol = door.DoorKnowledge switch
//     {
//         DoorKnowledge.Unknown     => (door.DoorOpenState == DoorOpenState.Unopened) ? UnknownSymbol : (char?)null,
//         DoorKnowledge.KnownWinner => WinnerSymbol,
//         DoorKnowledge.KnownLoser  => LoserSymbol,
//         _ => null
//     };
//     if (knowledgeSymbol.HasValue)
//     {
//         int centerCol = (interiorWidth / 2) + 1;
//         grid[2][centerCol] = knowledgeSymbol.Value;
//     }
//
//     // Top label (door number inside a 3-wide sign, centered)
//     string[] topLabel =
//     {
//         "┌───┐",
//         $"│ {number} │",
//         "└───┘"
//     };
//
//     // Pad top label to doorWidth, centered
//     var topLines = topLabel
//         .Select(l => l.PadLeft((doorWidth + l.Length) / 2).PadRight(doorWidth))
//         .ToArray();
//
//     // Bottom label: status marker
//     char statusMarker = door.DoorPickedState switch
//     {
//         DoorPickedState.PickedByPlayer => 'P',
//         DoorPickedState.PickedByHost   => 'H',
//         _                              => '·'
//     };
//     string bottomLabel = statusMarker.ToString().PadLeft(doorWidth / 2 + 1).PadRight(doorWidth);
//
//     // Build full block
//     var lines = new List<string>();
//     lines.AddRange(topLines);
//     lines.AddRange(grid.Select(row => new string(row)));
//     if (includeLabel) lines.Add(bottomLabel);
//
//     return lines.ToArray();
// }
//
//
// }
