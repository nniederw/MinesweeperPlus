namespace Minesweeper
{
    public class BoardChecker
    {
        public static (int x, int y) FindFirstZero(Board board)
        {
            for (int x = 0; x < board.SizeX; x++)
            {
                for (int y = 0; y < board.SizeY; y++)
                {
                    if (board.CheatGetNumbers()[x, y] == 0)
                    {
                        return (x, y);
                    }
                }
            }
            return (0, 0);
        }
        public static bool[,] CheckBoard(Board board, bool printProgress = false)
        {
            var sbs = new EdgeBruteForceBoardSolver(board);
            var result = new bool[board.SizeX, board.SizeY];
            for (int x = 0; x < board.SizeX; x++)
            {
                for (int y = 0; y < board.SizeY; y++)
                {
                    result[x, y] = sbs.IsSolvable(x, y);
                    if (printProgress)
                    {
                        var s = result[x, y] ? "solvable" : "not solvable";
                        Console.WriteLine($"Found square ({x},{y}) to be {s}.");
                    }
                }
            }
            return result;
        }
        public static bool[,] CheckBoard<BoardSolver>(Board board, bool printProgress = false) where BoardSolver : BaseBoardSolver, new()
        {
            var sbs = new BoardSolver();
            sbs.ChangeBoard(board);
            var result = new bool[board.SizeX, board.SizeY];
            for (int x = 0; x < board.SizeX; x++)
            {
                for (int y = 0; y < board.SizeY; y++)
                {
                    result[x, y] = sbs.IsSolvable(x, y);
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if both solvers gave the same output.</returns>
        public static bool SameSolvabilityTesterForSolvers<TestSolver, CompleteSolver>(Board board, bool printBoards = false) where TestSolver : BaseBoardSolver, new() where CompleteSolver : BaseBoardSolver, new()
        {
            //IBoardSolver testSolver = new TestSolver().Construct(board);
            IBoardSolver completeSolver = new CompleteSolver();
            completeSolver.ChangeBoard(board);
            if (completeSolver is BaseBoardSolver && ((BaseBoardSolver)completeSolver).GetSolvabilityClass != SolvabilityClass.Complete)
            {
                Console.WriteLine($"Comparing Solvability with a Solver that doesn't claim to be in the {nameof(SolvabilityClass)}.{nameof(SolvabilityClass.Complete)}, which is odd.");
            }
            var solvable = CheckBoard<TestSolver>(board);
            var solvable2 = CheckBoard<CompleteSolver>(board);
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
            if (printBoards && different)
            {
                Console.WriteLine($"{typeof(TestSolver).FullName}");
                BoardConverter.PrettyPrintBoard(board, solvable);
                Console.WriteLine();
                Console.WriteLine($"{typeof(CompleteSolver).FullName}");
                BoardConverter.PrettyPrintBoard(board, solvable2);
                Console.WriteLine();
            }
            return !different;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Solver"></typeparam>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="startPos"></param>
        /// <param name="verboseLogging"></param>
        /// <returns>Time of board 1 - time of board 2</returns>
        public static TimeSpan BoardComparer<Solver>(IBoard b1, IBoard b2, (int x, int y) startPos, bool withoutPrint = false, bool verboseLogging = false) where Solver : IBoardSolver, new()
        {
            Solver solver1 = new Solver();
            solver1.SetVerboseLogging(verboseLogging);
            solver1.ChangeBoard(b1);
            Solver solver2 = new Solver();
            solver2.SetVerboseLogging(verboseLogging);
            solver2.ChangeBoard(b2);
            var time = DateTime.Now;
            bool solvability1 = solver1.IsSolvable(startPos);
            var timeSolver1 = DateTime.Now - time;
            time = DateTime.Now;
            bool solvability2 = solver2.IsSolvable(startPos);
            var timeSolver2 = DateTime.Now - time;
            if (!withoutPrint)
            {
                Console.WriteLine($"Board 1: {timeSolver1.TotalMilliseconds}ms, Board 2: {timeSolver2.TotalMilliseconds}ms = {((double)timeSolver2.Ticks) / timeSolver1.Ticks * 100.0:F1}% of other board.");
            }
            if (solvability1 != solvability2)
            {
                Ext.ConsoleWriteColor($"Board 1 has solvability {solvability1}, but Board 2 has solvability {solvability2}", ConsoleColor.Red);
                Console.WriteLine();
            }
            return timeSolver1 - timeSolver2;
        }
        /// <returns>Time of solver 1 - time of solver 2</returns>
        public static TimeSpan SolverComparer<Solver1, Solver2>(Board b, (int x, int y) startPos, bool withoutPrint = false, bool verboseLogging = false) where Solver1 : IBoardSolver, new() where Solver2 : IBoardSolver, new()
        {
            Solver1 solver1 = new Solver1();
            solver1.SetVerboseLogging(verboseLogging);
            solver1.ChangeBoard(b);
            Solver2 solver2 = new Solver2();
            solver2.SetVerboseLogging(verboseLogging);
            solver2.ChangeBoard(b);
            var time = DateTime.Now;
            bool solvability1 = solver1.IsSolvable(startPos);
            var timeSolver1 = DateTime.Now - time;
            time = DateTime.Now;
            bool solvability2 = solver2.IsSolvable(startPos);
            var timeSolver2 = DateTime.Now - time;
            if (!withoutPrint)
            {
                Console.WriteLine($"{typeof(Solver1).Name}: {timeSolver1.TotalMilliseconds}ms, {typeof(Solver2).Name}: {timeSolver2.TotalMilliseconds}ms = {((double)timeSolver2.Ticks) / timeSolver1.Ticks * 100.0:F1}% of other solver.");
            }
            if (solvability1 != solvability2)
            {
                Ext.ConsoleWriteColor($"{typeof(Solver1).Name} has solvability {solvability1}, but {typeof(Solver2).Name} has solvability {solvability2}", ConsoleColor.Red);
                Console.WriteLine();
            }
            return timeSolver1 - timeSolver2;
        }
        public static void SolverTester<TestSolver, CompleteSolver>(bool printBoards = false) where TestSolver : BaseBoardSolver, new() where CompleteSolver : BaseBoardSolver, new()
        {
            bool foundDifference = false;
            uint completBoardTestCount = 0;
            foreach (var board in BoardGenerator.GetAllBoardsUpToSize(3, 3).Select(i => new Board(i)))
            {
                bool good = SameSolvabilityTesterForSolvers<TestSolver, CompleteSolver>(board, printBoards);
                if (!good)
                {
                    Console.WriteLine($"Board has different solvability for a board with values {board.SizeX},{board.SizeY},{board.Mines}.");
                    foundDifference = true;
                }
                Console.WriteLine($"Checked a total of {completBoardTestCount} boards so far, current size ({board.SizeX},{board.SizeY},{board.Mines})");
                completBoardTestCount++;
            }
            Console.WriteLine($"Finished with complete check boards, continueing with random boards.");
            const uint RandomTestCount = 200;
            for (int i = 0; i < RandomTestCount; i++)
            {
                var bboard = BoardGenerator.GetRandomSeededBoard(5, 5, 4, i);
                var board = new Board(bboard);
                bool good = SameSolvabilityTesterForSolvers<TestSolver, CompleteSolver>(board, printBoards);
                if (!good)
                {
                    Console.WriteLine($"Board has different solvability for random seeded board with values {board.SizeX},{board.SizeY},{board.Mines}, seed {i}.");
                    foundDifference = true;
                }
                Console.WriteLine($"Checked a total of {i} random boards so far, current size ({board.SizeX},{board.SizeY},{board.Mines}).");
            }
            if (!foundDifference)
            {
                Console.WriteLine($"Didn't find any different solvability in {RandomTestCount + completBoardTestCount} tested boards.");
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