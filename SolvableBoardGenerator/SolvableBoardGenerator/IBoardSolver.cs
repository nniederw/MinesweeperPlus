namespace Minesweeper
{
    public interface IBoardSolver
    {
        public IBoardSolver Construct(Board board, bool verboseLogging = false);
        public void ChangeBoard(Board board);
        public bool IsSolvable(int startX, int startY); //todo, probably better to replace it with empty constructors & a function to allow changing of boards
        public bool IsSolvable((int x, int y) startPos);
    }
}