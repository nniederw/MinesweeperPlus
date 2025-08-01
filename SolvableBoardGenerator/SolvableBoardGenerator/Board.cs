﻿namespace Minesweeper
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
        /// <summary>
        /// Note: mineField gets shallow copied.
        /// </summary>
        public Board(bool[,] mineField)
        {
            SizeX = Convert.ToUInt32(mineField.GetLength(0));
            SizeY = Convert.ToUInt32(mineField.GetLength(1));
            MineField = mineField;
            Mines = CalcMines();
            NumberField = new sbyte[SizeX, SizeY];
            CalcNumbers();
            NonMinesLeft = SizeX * SizeY - Mines;
            ClickedSquares = new bool[SizeX, SizeY];
        }
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
        public sbyte[,] CheatGetNumbers() => NumberField;
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