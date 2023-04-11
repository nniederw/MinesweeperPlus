namespace Minesweeper
{
    public interface IUnsolvedMineField
    {
        public int ClickSquare(int x, int y);
        public int ClickSquare((int x, int y) pos) => ClickSquare(pos.x, pos.y);
        public int[,] RevealedNumbers(); // -1 entry == not revealed; gives reference so updates automatically
        public int GetSizeX();
        public int GetSizeY();
        public int GetMineCount();
        public bool IsBlownUp();
    }
}