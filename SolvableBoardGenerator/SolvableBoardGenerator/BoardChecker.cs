namespace Minesweeper
{
    public class BoardChecker
    {
        public static bool[,] CheckBoard(Board board)
        {
            var sbs = new SimpleBoardSolver(board);
            var result = new bool[board.SizeX, board.SizeY];
            for (int x = 0; x < board.SizeX; x++)
            {
                for (int y = 0; y < board.SizeY; y++)
                {
                    result[x, y] = sbs.IsSolvable(x, y);
                }
            }
            return result;
        }
        public static void BoardBenchmarker()
        {
            TimeSpan totalTime = TimeSpan.Zero;
            uint solvedBoards = 0;
            for (int i = 0; i < 100; i++)
            {
                var bboard = BoardGenerator.GetRandomSeededBoard(6, 6, 4, i);
                var board = new Board(bboard);
                DateTime startTime = DateTime.Now;
                CheckBoard(board);
                totalTime += DateTime.Now - startTime;
                solvedBoards++;
                Console.WriteLine($"About {solvedBoards / totalTime.TotalSeconds} Boards solved per second.");
            }
        }
    }
}