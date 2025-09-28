namespace Minesweeper
{
    public struct BoardType
    {
        public uint Width;
        public uint Height;
        public uint Mines;
        public BoardType(uint width, uint height, uint mines)
        {
            Width = width; Height = height; Mines = mines;
        }
        public static BoardType Beginner => new BoardType(9, 9, 10);
        public static BoardType LegacyBeginner => new BoardType(8, 8, 10);
        public static BoardType Intermediate => new BoardType(16, 16, 40);
        public static BoardType Expert => new BoardType(30, 16, 99);
        public static BoardType Evil => new BoardType(30, 20, 130);
        public static BoardType SmallHighDensityTestBoard => new BoardType(5, 5, 8);
    }
}