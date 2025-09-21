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

//for: Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
//224, 1136, 1624, 1755, 1799 run out of memory
//10424 runs out of memory despite break early point of 25

List<((int x, int y) pos, int seed)> SolvableBoards = new List<((int x, int y) pos, int seed)> {
((0,0),121),
((0,3),3828),
((0,0),7865),
((0,0),9415),
((0,2),14399),
((0,0),15548),
((0,3),16629),
((0,4),30775),
((0,0),31799),
((0,0),35750),
((0,5),38850),
((0,3),42540),
((0,0),54338),
((0,0),56017),
((0,8),58892),
((0,0),68689),
((0,0),70812),
};
SolvableBoards.Clear();
foreach (var solvabel in SolvableBoards)
{
    Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), solvabel.seed);
    int x, y;
    (x, y) = solvabel.pos;
    BoardChecker.SolverComparer<EfficientSmartPermutationBuilderBoardSolver, SmarterPermutationBuilderBoardSolver>(b, solvabel.pos, false);
    /*SmartPermutationBuilderBoardSolver pSolver = new SmartPermutationBuilderBoardSolver(b, false);
    var time = DateTime.Now;
    bool solvableFromCurPos = pSolver.IsSolvable(x, y);
    var solveTime = DateTime.Now - time;
    EfficientSmartPermutationBuilderBoardSolver epSolver = new EfficientSmartPermutationBuilderBoardSolver(b, false);
    time = DateTime.Now;
    bool solvableFromCurPos2 = epSolver.IsSolvable(x, y);
    var solveTime2 = DateTime.Now - time;
    SmarterPermutationBuilderBoardSolver sSolver = new SmarterPermutationBuilderBoardSolver(b, false);
    //sSolver.DisableAutoLoggingDuringPhases();
    time = DateTime.Now;
    bool solvableFromCurPos3 = sSolver.IsSolvable(x, y);
    var solveTime3 = DateTime.Now - time;
    Console.WriteLine($"Solvetime Normal: {solveTime.TotalMilliseconds}ms, solvetime efficient: {solveTime2.TotalMilliseconds}ms, solvetime smarter: {solveTime3.TotalMilliseconds}ms, solvable normal: {solvableFromCurPos}, solvable efficient: {solvableFromCurPos2}, solvable smarter: {solvableFromCurPos3}");*/
}
//Console.ReadLine();
List<string> solvableBoards = new List<string>();
TimeSpan timeDifference = TimeSpan.Zero;
bool firstTime = true;
for (int i = 0; i < 0; i++)
{
    //Board b = BoardGenerator.GetRandomSeededBoard(BoardType.Expert, i);
    Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
    //Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(200, 200, 5000), i);
    var startPos = BoardChecker.FindFirstZero(b);

    TimeSpan t;
    if (i % 2 == 1)
    {
        Console.Write("B,BAB:");
        t = BoardChecker.BoardComparer<SmarterPermutationBuilderBoardSolver>(b, b.ToBitArrayBoard(), startPos);
    }
    else
    {
        Console.Write("BAB,B:");
        t = BoardChecker.BoardComparer<SmarterPermutationBuilderBoardSolver>(b.ToBitArrayBoard(), b, startPos);
        t = t * -1.0;
    }
    if (firstTime)
    {
        firstTime = false;
        continue;
    }
    timeDifference += t;
    if (i % 10 == 0)
    {
        Console.WriteLine($"Current time saving of BitArrayBoard: {timeDifference.TotalMilliseconds}ms (positive = BitArrayBoard is faster)");
    }
}
timeDifference = TimeSpan.Zero;
firstTime = true;
bool switchEvenOdd = false;
for (int i = 0; i < 0; i++)
{
    Board b = BoardGenerator.GetRandomSeededBoard(BoardType.Expert, i);
    //Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
    //Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(200, 200, 5000), i);
    var startPos = BoardChecker.FindFirstZero(b);
    TimeSpan t;
    if (!switchEvenOdd || i % 2 == 0)
    {
        t = BoardChecker.SolverComparer<EfficientSmartPermutationBuilderBoardSolver, SmarterPermutationBuilderBoardSolver>(b, startPos);
    }
    else
    {
        t = BoardChecker.SolverComparer<SmarterPermutationBuilderBoardSolver, EfficientSmartPermutationBuilderBoardSolver>(b, startPos);
        t = t * -1.0;
    }
    if (firstTime)
    {
        firstTime = false;
        continue;
    }
    timeDifference += t;
    if (i % 10 == 0)
    {
        Console.WriteLine($"Current time saving of SmarterPermutationBuilderBoardSolver: {timeDifference.TotalMilliseconds}ms (positive = SmarterPermutationBuilderBoardSolver is faster)");
    }
}
var lastTime = DateTime.Now;
const uint update = 2000;
for (int i = 0; i < 10000000; i++)
{
    //Board b = BoardGenerator.GetRandomSeededBoard(BoardType.Expert, i);
    Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(100, 100, 2100), i);
    //Board b = BoardGenerator.GetRandomSeededBoard(new BoardType(200, 200, 5000), i);
    var startPos = BoardChecker.FindFirstZero(b);
    var solver = new SmarterPermutationBuilderBoardSolver(b, false);
    solver.DisableAutoLoggingDuringPhases();
    var time = DateTime.Now;
    var solvable = solver.IsSolvable(startPos);
    var solveTime = DateTime.Now - time;
    if (solveTime.TotalSeconds > 1.0)
    {
        Console.WriteLine($"Took {solveTime.TotalSeconds}s for board {i}");
    }
    if (i % update == 0)
    {
        Console.WriteLine($"Checked up to board {i}, took {(DateTime.Now - lastTime).TotalSeconds}s, since last update.");
        lastTime = DateTime.Now;
    }
    if (solvable)
    {
        Console.WriteLine($"Solvable board found [seed,(x,y)]: {i},({startPos.x},{startPos.y})");
    }
}