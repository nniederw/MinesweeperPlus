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


//BoardChecker.SolverTester<EdgeBruteForceBoardSolver, BruteForceBoardSolver>(true);
//BoardChecker.SolverTester<SimpleBoardSolver, BruteForceBoardSolver>(true);
Console.OutputEncoding = System.Text.Encoding.UTF8;
//Board board = BoardGenerator.GetRandomSeededBoard(new BoardType(100,100,2100), 1);
//PermutationBuilderBoardSolver solver = new PermutationBuilderBoardSolver(board, true);
//PermutationBuilderBoardSolver fastSolver = new PermutationBuilderBoardSolver(board);
//BoardConverter.PrettyPrintBoard(board);
//solver.IsSolvable(0, 0);
//solver.IsSolvable(15, 0);
//var t = DateTime.Now;
//fastSolver.IsSolvable(15, 0);
//Console.WriteLine($"Took {(DateTime.Now-t).TotalMilliseconds}ms to finish the board.");
//224, 1136, 1624, 1755, 1799 run out of memory
//10424 runs out of memory despite break early point of 25
List<string> solvableBoards = new List<string>();
for (int i = 21569; i < 1000000; i++)
{
    Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
    //BoardConverter.PrettyPrintBoard(b);
    SmartPermutationBuilderBoardSolver pSolver = new SmartPermutationBuilderBoardSolver(b, false);
    pSolver.SetBreakEarlyLogicChain(25);
    pSolver.SetMaxMergablePermutationCount(100000);
    pSolver.DisableAutoLoggingDuringPhases();
    var time = DateTime.Now;
    int x, y;
    (x, y) = BoardChecker.FindFirstZero(b);
    bool solvableFromCurPos = pSolver.IsSolvable(x, y);
    var solveTime = DateTime.Now - time;
    if (solvableFromCurPos)
    {
        solvableBoards.Add($"Board {i} is solvable at ({x},{y}), solved in {solveTime.TotalMilliseconds}ms.");
        Console.WriteLine();
        foreach (string s in solvableBoards)
        {
            Ext.ConsoleWriteColor(s, ConsoleColor.Red);
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine($"Board {i} isn't solvable at ({x},{y}), finished in {solveTime.TotalMilliseconds}ms. Was able to clear {pSolver.PercentageCleared}%");
    }
}
/*
for (int i = 3; i < 2000; i++)
{
    Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
    int x, y;
    (x, y) = BoardChecker.FindFirstZero(b);
    SmartPermutationBuilderBoardSolver pSolver = new SmartPermutationBuilderBoardSolver(b, true);
    pSolver.DisableAutoLoggingDuringPhases();
    var time = DateTime.Now;
    bool solvableFromCurPos = pSolver.IsSolvable(x, y);
    var solveTime = DateTime.Now - time;
    if (solvableFromCurPos)
    {
        Console.WriteLine($"Board {i} is solvable at ({x},{y}), solved in {solveTime.TotalMilliseconds}ms:");
        //BoardConverter.PrettyPrintBoard(b);
        //var pS = new PermutationBuilderBoardSolver(b, true);
        //pS.IsSolvable(x, y);
    }
    Console.WriteLine($"Board {i} isn't solvable at ({x},{y}), finished in {solveTime.TotalMilliseconds}ms:");
}*/

/*for (int i = 0; i < 2000; i++)
{
    Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
    bool solvable = false;
    for (int x = 0; x < b.SizeX; x++)
    {
        for (int y = 0; y < b.SizeY; y++)
        {
            if (b.CheatGetNumbers()[x, y] == 0)
            {
                PermutationBuilderBoardSolver pSolver = new PermutationBuilderBoardSolver(b);
                var time = DateTime.Now;
                bool solvableFromCurPos = pSolver.IsSolvable(x, y);
                var solveTime = DateTime.Now - time;
                if (solvableFromCurPos)
                {
                    solvable = true;
                    Console.WriteLine($"Board {i} is solvable at ({x},{y}), solved in {solveTime.TotalMilliseconds}ms:");
                    //BoardConverter.PrettyPrintBoard(b);
                    //var pS = new PermutationBuilderBoardSolver(b, true);
                    //pS.IsSolvable(x, y);
                    break;
                }
            }
        }
        if (solvable) { break; }
    }
    if (!solvable)
    {
        Console.WriteLine($"Board {i} wasn't solvable");
    }
}*/
//Board board = BoardGenerator.GetRandomSeededBoard(BoardType.Expert, 82);
//Board board = BoardGenerator.GetRandomSeededBoard(BoardType.Expert, 42);
//BoardConverter.PrettyPrintBoard(board);
//EdgeBruteForceBoardSolver solver = new EdgeBruteForceBoardSolver(board, true);
//PermutationBuilderBoardSolver solver = new PermutationBuilderBoardSolver(board, true);
//solver.IsSolvable(14, 0);
//solver.IsSolvable(15, 9);
//solver.PrintCurrentStateBoard();
//Board board = new Board(BoardGenerator.GetRandomSeededBoard(20, 30, 99, 0));
/*
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
*/