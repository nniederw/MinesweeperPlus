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
/*
var bfield = BoardConverter.BinaryIntsToBools(field);
Board board = new Board(bfield);
BoardConverter.PrintBoolArray(bfield);
Console.WriteLine();
var solvables = BoardChecker.CheckBoard(board);
BoardConverter.PrintBoolArray(solvables);*/
/*var boards = BoardGenerator.GetAllBoardsOfSize(6, 6).Skip(2000);
foreach (var board in boards)
{
    Console.WriteLine();
    //Console.WriteLine("Board:");
    //BoardConverter.PrintBoolArray(board);
    Board b = new Board(board);
    //Console.WriteLine();
    //Console.WriteLine("Solvable:");
    var solvables = BoardChecker.CheckBoard(b);
    BoardConverter.PrettyPrintBoard(b, solvables);
    //BoardConverter.PrintBoolArray(solvables);
    Console.WriteLine();
}*/
/*var boards = new List<bool[,]>();
for (int i = 0; i < 10; i++)
{
    boards.Add(BoardGenerator.GetRandomSeededBoard(5, 5, 3));
}
foreach (var board in boards)
{
    Console.WriteLine();
    //Console.WriteLine("Board:");
    //BoardConverter.PrintBoolArray(board);
    Board b = new Board(board);
    //Console.WriteLine();
    //Console.WriteLine("Solvable:");
    var solvables = BoardChecker.CheckBoard(b);
    BoardConverter.PrettyPrintBoard(b, solvables);
    //BoardConverter.PrintBoolArray(solvables);
    Console.WriteLine();
}*/
BoardChecker.BoardBenchmarker();