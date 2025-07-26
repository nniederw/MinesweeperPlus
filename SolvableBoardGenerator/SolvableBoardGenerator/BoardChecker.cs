namespace Minesweeper
{
    public class BoardChecker
    {
        public static bool[,] CheckBoard(Board board, bool fastBruteForce = false, bool printProgress = false)
        {
            var sbs = new SimpleBoardSolver(board);
            var result = new bool[board.SizeX, board.SizeY];
            for (int x = 0; x < board.SizeX; x++)
            {
                for (int y = 0; y < board.SizeY; y++)
                {
                    result[x, y] = sbs.IsSolvable(x, y, fastBruteForce);
                    if (printProgress)
                    {
                        var s = result[x, y] ? "solvable" : "not solvable";
                        Console.WriteLine($"Found square ({x},{y}) to be {s}.");
                    }
                }
            }
            return result;
        }
        public static void BoardBenchmarker(bool printBoards = false)
        {
            TimeSpan totalTime = TimeSpan.Zero;
            uint solvedBoards = 0;
            for (int i = 0; i < 100; i++)
            {
                var bboard = BoardGenerator.GetRandomSeededBoard(6, 6, 4, i);
                var board = new Board(bboard);
                //Console.WriteLine();
                //BoardConverter.PrettyPrintBoard(board);
                //Console.WriteLine();
                DateTime startTime = DateTime.Now;
                var solvable = CheckBoard(board);
                totalTime += DateTime.Now - startTime;
                solvedBoards++;
                if (printBoards)
                {
                    Console.WriteLine();
                    BoardConverter.PrettyPrintBoard(board, solvable);
                    Console.WriteLine();
                }
                Console.WriteLine($"About {solvedBoards / totalTime.TotalSeconds} Boards solved per second.");
            }
        }
        public static void SolverTester(bool printBoards = false)
        {
            const uint TestCount = 1000;
            bool foundDifference = false;
            for (int i = 0; i < TestCount; i++)
            {
                var bboard = BoardGenerator.GetRandomSeededBoard(5, 5, 4, i);
                var board = new Board(bboard);
                //Console.WriteLine();
                //BoardConverter.PrettyPrintBoard(board);
                //Console.WriteLine();
                var solvable = CheckBoard(board);
                var solvable2 = CheckBoard(board, true);
                bool different = false;
                for (int x = 0; x < board.SizeX; x++)
                {
                    for (int y = 0; y < board.SizeY; y++)
                    {
                        if (solvable[x, y] != solvable2[x, y])
                        {
                            different = true;
                            Console.WriteLine($"Differnent solvability for solvers at ({x},{y})");
                        }
                    }
                }
                if (different)
                {
                    foundDifference = true;
                    Console.WriteLine($"Board has different solvability for random seeded board with values {4},{4},{3},{i}.");
                }
                if (printBoards && different)
                {
                    Console.WriteLine("Normal brute force");
                    BoardConverter.PrettyPrintBoard(board, solvable);
                    Console.WriteLine();
                    Console.WriteLine("Fast brute force");
                    BoardConverter.PrettyPrintBoard(board, solvable2);
                    Console.WriteLine();
                }
            }
            if (!foundDifference)
            {
                Console.WriteLine($"Didn't find any different solvability in {TestCount} tested boards.");
            }
        }
    }
}