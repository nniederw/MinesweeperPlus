using System.Collections.Generic;
using System.Linq;
namespace Minesweeper
{
    public class BoardSolver
    {
        private int SizeX;
        private int SizeY;
        private int Mines;
        public BoardSolver(int sizeX, int sizeY, int mines)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Mines = mines;
        }
        public bool IsSolvable(int seed, int startX, int startY)
         => IsSolvable(Board.GenerateField(seed, SizeX, SizeY, Mines), startX, startY);
        public bool IsSolvable(int[,] mineField, int startX, int startY)
        {
            bool[,] clearSquares = new bool[SizeX, SizeY];
            bool[,] mines = new bool[SizeX, SizeY];
            HashSet<(int x, int y)> active = new HashSet<(int x, int y)>();
            if (mineField[startX, startY] == 9)
            {
                return false;
            }
            if (mineField[startX, startY] != 0)
            {
                return AdjPos((startX, startY)).Count == Mines;
            }
            ClearSquare(startX, startY);
            bool changed = false;
            do
            {
                var squares = active.ToList();
                foreach (var s in squares)
                {
                    changed = changed || TestSquare(s);
                }
            } while (changed);

            /*
             * 1: 1 sized patterns                  
             * 2: calculate all regions
             * 3: iterate over mine possibilities
             */
            return active.Count == 0;
            bool TestSquare((int x, int y) pos)
            {
                var unknowns = AdjUnknownPos(pos);
                int number = mineField[pos.x, pos.y];
                if (unknowns.Count == number)
                {
                    unknowns.ForEach(SetMine);
                    return true;
                }
                var certainMines = AdjCertainMines(pos);
                if (certainMines.Count == number)
                {
                    unknowns.Intersect(certainMines).Foreach(i => ClearSquare(i.Item1, i.Item2));
                    return true;
                }
                return false;
            }
            void ClearSquare(int x, int y)
            {
                active.Add((x, y));
                if (!clearSquares[x, y])
                {
                    clearSquares[x, y] = true;
                    if (mineField[x, y] == 0)
                    {
                        AdjPos((x, y)).ForEach(i => ClearSquare(i.Item1, i.Item2));
                        active.Remove((x, y));
                    }
                    else
                    {
                        AdjPos((x, y)).ForEach(CheckActive);
                    }
                }
            }
            void CheckActive((int x, int y) pos)
            {
                if (AdjPos(pos).TrueForAll((i) =>
                 mines[i.Item1, i.Item2] || clearSquares[i.Item1, i.Item2]))
                {
                    active.Add(pos);
                }
                else
                {
                    if (active.Contains(pos))
                    {
                        active.Remove(pos);
                    }
                }
            }
            void SetMine((int x, int y) pos)
            {
                mines[pos.x, pos.y] = true;
                AdjPos(pos).ForEach(CheckActive);
            }
            List<(int, int)> AdjPos((int x, int y) pos)
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
                return res.Where((i) => mineField.InBound(i.Item1, i.Item2)).ToList();
            }
            List<(int, int)> AdjUnknownPos((int x, int y) pos)
                => AdjPos(pos).Where(i => !clearSquares[i.Item1, i.Item2]).ToList();
            List<(int, int)> AdjCertainMines((int x, int y) pos)
                => AdjPos(pos).Where(i => mines[i.Item1, i.Item2]).ToList();
        }
    }
}