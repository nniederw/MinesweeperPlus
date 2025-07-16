using Minesweeper;

int[,] field;
/*
field = (new int[,] {
            { 0, 1, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }});
*/

field = (new int[,] {
            { 0, 0, 1, 1 },
            { 0, 0, 0, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 1, 1 },
            { 0, 0, 0, 1 }});

var bfield = BoardConverter.BinaryIntsToBools(field);
Board board = new Board(bfield);
BoardConverter.PrintBoolArray(bfield);
Console.WriteLine();
var solvables = BoardChecker.CheckBoard(board);
BoardConverter.PrintBoolArray(solvables);