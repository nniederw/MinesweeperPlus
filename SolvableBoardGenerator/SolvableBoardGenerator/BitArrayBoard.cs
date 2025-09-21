namespace Minesweeper
{
    public class BitArrayBoard : IBoard
    {
        public uint SizeX { private set; get; }
        public uint SizeY { private set; get; }
        public uint Mines { private set; get; }
        private BitArray2D MineField;
        private sbyte[,] NumberField; //0-8 numbers, 9 meaning mine
        private uint NonMinesLeft;
        private BitArray2D ClickedSquares;
        private List<(int x, int y)> StartClears = new List<(int x, int y)>();
        public const sbyte MineSByte = 9;
        public BitArrayBoard(bool[,] mineField, List<(int x, int y)> startClears = null)
            : this(new BitArray2D(mineField), startClears) { }
        /// <summary>
        /// Note: mineField is used for the internal representation, please don't modify it after parsing it.
        /// </summary>
        public BitArrayBoard(BitArray2D mineField, List<(int x, int y)> startClears = null)
        {
            SizeX = mineField.Length1;
            SizeY = mineField.Length2;
            MineField = mineField;
            Mines = CalcMines();
            NumberField = new sbyte[SizeX, SizeY];
            CalcNumbers();
            ClickedSquares = new BitArray2D(SizeX, SizeY);
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
        public IEnumerable<(int x, int y)> GetNeighbors((int x, int y) pos) => GetNeighbors(pos.x, pos.y);
        public IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            if (0 < x && x < SizeX - 1 && 0 < y && y < SizeY - 1)
            {
                yield return (x - 1, y + 1);
                yield return (x + 0, y + 1);
                yield return (x + 1, y + 1);
                yield return (x + 1, y + 0);
                yield return (x + 1, y - 1);
                yield return (x + 0, y - 1);
                yield return (x - 1, y + 0);
                yield return (x - 1, y - 1);
            }
            else
            {
                if (InBoardBound(x - 1, y + 1)) { yield return (x - 1, y + 1); }
                if (InBoardBound(x + 0, y + 1)) { yield return (x + 0, y + 1); }
                if (InBoardBound(x + 1, y + 1)) { yield return (x + 1, y + 1); }
                if (InBoardBound(x + 1, y + 0)) { yield return (x + 1, y + 0); }
                if (InBoardBound(x + 1, y - 1)) { yield return (x + 1, y - 1); }
                if (InBoardBound(x + 0, y - 1)) { yield return (x + 0, y - 1); }
                if (InBoardBound(x - 1, y + 0)) { yield return (x - 1, y + 0); }
                if (InBoardBound(x - 1, y - 1)) { yield return (x - 1, y - 1); }
            }
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
                        NumberField[x, y] = MineSByte;
                    }
                    else
                    {
                        NumberField[x, y] = Convert.ToSByte(GetNeighbors(x, y).Count(i => MineField[i.x, i.y]));
                    }
                }
            }
        }
        private bool InBoardBound(int x, int y)
            => 0 <= x && x < SizeX && (0 <= y && y < SizeY);
        private bool XInBoardBound(int x)
            => 0 <= x && x < SizeX;
        private bool YInBoardBound(int y)
            => 0 <= y && y < SizeY;
    }
}