namespace Minesweeper
{
    public class MineExplosionException : Exception
    {
        public MineExplosionException() : base() { }
        public MineExplosionException(string s) : base(s) { }
    }
    public class Board
    {
        public uint SizeX { private set; get; }
        public uint SizeY { private set; get; }
        public uint Mines { private set; get; }
        private bool[,] MineField;
        private sbyte[,] NumberField; //0-8 numbers, 9 meaning mine
        private uint NonMinesLeft;
        private bool[,] ClickedSquares;
        private List<(int x, int y)> StartClears = new List<(int x, int y)>();
        /// <summary>
        /// Note: mineField gets copied.
        /// </summary>
        public Board(bool[,] mineField, List<(int x, int y)> startClears = null)
        {
            SizeX = Convert.ToUInt32(mineField.GetLength(0));
            SizeY = Convert.ToUInt32(mineField.GetLength(1));
            MineField = mineField;
            Mines = CalcMines();
            NumberField = new sbyte[SizeX, SizeY];
            CalcNumbers();
            ClickedSquares = new bool[SizeX, SizeY];
            if (startClears != null)
            {
                StartClears = startClears;
            }
            ResetBoard();
        }
        public sbyte ClickOnSquare((int x, int y) pos) => ClickOnSquare(pos.x, pos.y);
        public sbyte ClickOnSquare(int x, int y)
        {
            if (MineField[x, y]) throw new MineExplosionException($"Clicked on a mine on ({x}, {y})");
            if (!ClickedSquares[x, y])
            {
                ClickedSquares[x, y] = true;
                NonMinesLeft--;
            }
            return NumberField[x, y];
        }
        public void ResetBoard()
        {
            NonMinesLeft = SizeX * SizeY - Mines;
            for (uint x = 0; x < SizeX; x++)
            {
                for (uint y = 0; y < SizeY; y++)
                {
                    ClickedSquares[x, y] = false;
                }
            }
            try
            {
                StartClears.ForEach(pos => ClickOnSquare(pos));
            }
            catch (MineExplosionException mee) { throw new Exception($"The {nameof(Board)} had {nameof(StartClears)} with mine position making it explode on board generation."); }
        }
        public bool IsCleared() => NonMinesLeft == 0;
        public List<(int x, int y)> GetNeighbors((int x, int y) pos) => GetNeighbors(pos.x, pos.y);
        public List<(int x, int y)> GetNeighbors(int x, int y)
        {
            List<(int x, int y)> result = new();
            if (InBoardBound(x - 1, y + 1)) { result.Add((x - 1, y + 1)); }
            if (InBoardBound(x + 0, y + 1)) { result.Add((x + 0, y + 1)); }
            if (InBoardBound(x + 1, y + 1)) { result.Add((x + 1, y + 1)); }
            if (InBoardBound(x + 1, y + 0)) { result.Add((x + 1, y + 0)); }
            if (InBoardBound(x + 1, y - 1)) { result.Add((x + 1, y - 1)); }
            if (InBoardBound(x + 0, y - 1)) { result.Add((x + 0, y - 1)); }
            if (InBoardBound(x - 1, y + 0)) { result.Add((x - 1, y + 0)); }
            if (InBoardBound(x - 1, y - 1)) { result.Add((x - 1, y - 1)); }
            return result;
        }
        public IEnumerable<(int x, int y)> AllSquares()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    yield return (x, y);
                }
            }
        }
        public IEnumerable<(int x, int y)> GetStartClears() => StartClears;
        public sbyte[,] CheatGetNumbers() => NumberField;
        public bool IsEmpty => SizeX == 0 && SizeY == 0;
        public static Board GetEmptyBoard() => new Board(new bool[0, 0]);
        private uint CalcMines()
        {
            uint mines = 0;
            for (uint x = 0; x < SizeX; x++)
            {
                for (uint y = 0; y < SizeY; y++)
                {
                    if (MineField[x, y])
                    {
                        mines++;
                    }
                }
            }
            return mines;
        }
        private void CalcNumbers()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    if (MineField[x, y])
                    {
                        NumberField[x, y] = 9;
                    }
                    else
                    {
                        NumberField[x, y] = Convert.ToSByte(GetNeighbors(x, y).Count(i => MineField[i.x, i.y]));
                    }
                }
            }
        }
        private bool InBoardBound(int x, int y)
        {
            return (0 <= x && x < SizeX) && (0 <= y && y < SizeY);
        }
    }
}