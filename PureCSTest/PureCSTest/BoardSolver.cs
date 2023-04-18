using System.Collections.Generic;
using System.Linq;
namespace Minesweeper
{
    public class BoardSolver
    {
        private int SizeX;
        private int SizeY;
        private int MineCount;
        private bool[,] ClearSquares;
        private bool[,] Mines;
        private sbyte[,] Numbers;
        private IUnsolvedMineField UnsolvedField;
        public BoardSolver(IUnsolvedMineField unsolvedField)
        {
            SizeX = unsolvedField.GetSizeX();
            SizeY = unsolvedField.GetSizeY();
            MineCount = unsolvedField.GetMineCount();
            UnsolvedField = unsolvedField;
        }
        //public bool IsSolvable(int seed, int startX, int startY)
        // => IsSolvable(Board.GenerateField(seed, SizeX, SizeY, Mines), startX, startY);
        public bool IsSolvable(int startX, int startY)
        {
            ClearSquares = new bool[SizeX, SizeY];
            Mines = new bool[SizeX, SizeY];
            Numbers = UnsolvedField.RevealedNumbers();
            HashSet<(int x, int y)> active = new HashSet<(int x, int y)>();
            ClearSquare(startX, startY);
            if (UnsolvedField.IsBlownUp()) return false;
            if (Numbers[startX, startY] != 0)
            {
                return AdjPos((startX, startY)).Count == MineCount;
            }
            bool changed = false;
            do
            {
                changed = false;
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
                int number = Numbers[pos.x, pos.y];
                if (unknowns.Count == number)
                {
                    unknowns.ForEach(SetMine);
                    return true;
                }
                var certainMines = AdjCertainMines(pos);
                if (certainMines.Count == number)
                {
                    unknowns.Except(certainMines).Foreach(i => ClearSquare(i.Item1, i.Item2));
                    return true;
                }
                return false;
            }
            void ClearSquare(int x, int y)
            {
                active.Add((x, y));
                if (!ClearSquares[x, y])
                {
                    ClearSquares[x, y] = true;
                    if (UnsolvedField.ClickSquare(x, y) == 0)
                    {
                        AdjPos((x, y)).Where(i => !ClearSquares[i.Item1, i.Item2]).Foreach(i => ClearSquare(i.Item1, i.Item2));
                        active.Remove((x, y));
                    }
                    if (Numbers[x, y] == 9) throw new Exception();
                    AdjPos((x, y)).ForEach(CheckActive);
                    CheckActive((x, y));
                }
            }
            void CheckActive((int x, int y) pos)
            {
                if (AdjPos(pos).TrueForAll((i) =>
                 Mines[i.Item1, i.Item2] || ClearSquares[i.Item1, i.Item2]))
                {
                    if (active.Contains(pos))
                    {
                        active.Remove(pos);
                    }
                }
            }
            void SetMine((int x, int y) pos)
            {
                Mines[pos.x, pos.y] = true;
                CheckActive(pos);
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
                return res.Where((i) => Numbers.InBound(i.Item1, i.Item2)).ToList();
            }
            List<(int, int)> AdjUnknownPos((int x, int y) pos)
                => AdjPos(pos).Where(i => !ClearSquares[i.Item1, i.Item2]).ToList();
            List<(int, int)> AdjCertainMines((int x, int y) pos)
                => AdjPos(pos).Where(i => Mines[i.Item1, i.Item2]).ToList();
        }
        private void PrintBoard(int[,] numbers, bool[,] mines, bool[,] clearSquares)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            for (int x = 0; x < numbers.GetLength(0); x++)
            {
                for (int y = 0; y < numbers.GetLength(0); y++)
                {
                    string c = mines[x, y] ? "*" : numbers[x, y] == -1 ? "." : numbers[x, y].ToString();
                    Console.BackgroundColor = clearSquares[x, y] ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write($"{c} ");
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }
}