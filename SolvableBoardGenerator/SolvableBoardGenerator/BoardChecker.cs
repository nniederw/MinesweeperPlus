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
    }
}