namespace Minesweeper
{
    public interface IBoard
    {
        public uint SizeX { get; }
        public uint SizeY { get; }
        public uint Mines { get; }
        public sbyte ClickOnSquare((int x, int y) pos);
        public sbyte ClickOnSquare(int x, int y);
        public void ResetBoard();
        public bool IsCleared();
        public IEnumerable<(int x, int y)> GetNeighbors((int x, int y) pos);
        public IEnumerable<(int x, int y)> GetNeighbors(int x, int y);
        public IEnumerable<(int x, int y)> AllSquares();
        public IEnumerable<(int x, int y)> GetStartClears();
        public sbyte[,] CheatGetNumbers();
        public bool IsEmpty { get; }
    }
}