using System;
using System.Collections.Generic;
using System.Linq;
namespace Minesweeper
{
    public class UnsolvedMineField : IUnsolvedMineField
    {
        private int[,] Numbers;
        private int MineCount;
        private int[,] OpenedSquares;
        private bool BlownUp = false;
        public UnsolvedMineField(int seed, int sizeX, int sizeY, int mines)
        {
            Numbers = Board.GenerateField(seed, sizeX, sizeY, mines);
            OpenedSquares = new int[sizeX, sizeY];
            OpenedSquares.Fill(-1);
        }
        public UnsolvedMineField(int[,] numbers)
        {
            Numbers = numbers;
            MineCount = Numbers.SingleDArray().Count(i => i == 9);
            OpenedSquares = new int[GetSizeX(), GetSizeY()];
            OpenedSquares.Fill(-1);
        }
        public int ClickSquare((int x, int y) pos) => ClickSquare(pos.x, pos.y);
        public int ClickSquare(int x, int y)
        {
            if (Numbers[x, y] == 9)
            {
                BlownUp = true;
                return 9;
            }
            if (OpenedSquares[x, y] == -1)
            {
                OpenedSquares[x, y] = Numbers[x, y];
                if (Numbers[x, y] == 0)
                {
                    AdjPos((x, y)).Where(i => OpenedSquares[i.Item1, i.Item2] == -1).Foreach(i => ClickSquare(i));
                }
            }
            return Numbers[x, y];
        }
        public int[,] RevealedNumbers() => OpenedSquares;
        public int GetSizeX() => Numbers.GetLength(0);
        public int GetSizeY() => Numbers.GetLength(1);
        public int GetMineCount() => MineCount;
        public bool IsBlownUp() => BlownUp;
        private List<(int, int)> AdjPos((int x, int y) pos)
        {
            (int x, int y) = pos;
            var res = new List<(int, int)>();
            res.Add((x - 1, y + 1));
            res.Add((x + 0, y + 1));
            res.Add((x + 1, y + 1));
            res.Add((x + 1, y + 0));
            res.Add((x + 1, y - 1));
            res.Add((x + 0, y - 1));
            res.Add((x - 1, y - 1));
            res.Add((x - 1, y + 0));
            return res.Where((i) => Numbers.InBound(i.Item1, i.Item2)).ToList();
        }
    }
}