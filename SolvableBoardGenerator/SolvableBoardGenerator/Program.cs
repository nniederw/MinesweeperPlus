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
//BoardChecker.BoardBenchmarker(true);

//BoardChecker.SolverTester(true);
/*
Board board = new Board(BoardGenerator.GetRandomSeededBoard(4, 4, 3, 18));
BoardConverter.PrettyPrintBoard(board);
var s = BoardChecker.CheckBoard(board);
var s2 = BoardChecker.CheckBoard(board, true);
BoardConverter.PrettyPrintBoard(board,s);
Console.WriteLine();
BoardConverter.PrettyPrintBoard(board,s2);
*/

//Board board = new Board(BoardGenerator.GetRandomSeededBoard(20, 30, 99, 0));
const uint width = 16;
const uint height = 16;
const uint mines = 25;
int[,] solvableScore = new int[16, 16];
for (int i = 100030; i < 5000000; i++)
{
    var bboard = BoardGenerator.GetRandomSeededBoard(width, height, mines, i);
    Board board = new Board(bboard);
    //BoardConverter.PrettyPrintBoard(board);
    var solvable = BoardChecker.CheckBoard(board, true);
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            if (bboard[x, y]) { continue; }
            solvableScore[x, y] += solvable[x, y] ? 1 : -1;
        }
    }

    if (i % 30 == 0)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Console.Write($"{solvableScore[x, y]},");
            }
            Console.WriteLine();
        }
    }
    BoardConverter.PrettyPrintBoard(board, solvable);
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
    Console.WriteLine($"Checked board {i}");

}
Console.WriteLine("End reached");
for (int x = 0; x < width; x++)
{
    for (int y = 0; y < height; y++)
    {
        Console.Write($"{solvableScore[x, y]},");
    }
    Console.WriteLine();
}

var wait = Console.ReadLine();