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
        HashSet<(int x, int y)> Active = new HashSet<(int x, int y)>();
        private HashSet<MineRegion> Regions = new HashSet<MineRegion>();
        private List<MineRegion>[,] RegionsOnSquare;
        private IUnsolvedMineField UnsolvedField;
        public BoardSolver(IUnsolvedMineField unsolvedField)
        {
            SizeX = unsolvedField.GetSizeX();
            SizeY = unsolvedField.GetSizeY();
            MineCount = unsolvedField.GetMineCount();
            UnsolvedField = unsolvedField;
            ClearSquares = new bool[SizeX, SizeY];
            Mines = new bool[SizeX, SizeY];
            Numbers = new sbyte[SizeX, SizeY];
            RegionsOnSquare = new List<MineRegion>[SizeX, SizeY];
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    RegionsOnSquare[x, y] = new List<MineRegion>();
                }
            }
        }
        //public bool IsSolvable(int seed, int startX, int startY)
        // => IsSolvable(Board.GenerateField(seed, SizeX, SizeY, Mines), startX, startY);
        public bool IsSolvable(int startX, int startY)
        {
            /*
             * 1: 1 sized patterns                  
             * 2: calculate all regions
             * 3: iterate over mine possibilities
             */
            ClearSquares = new bool[SizeX, SizeY];
            Mines = new bool[SizeX, SizeY];
            Numbers = UnsolvedField.RevealedNumbers();
            Active.Clear();
            Regions.Clear();
            ClearSquare(startX, startY);
            if (UnsolvedField.IsBlownUp()) return false;
            if (Numbers[startX, startY] != 0)
            {
                return AdjPos((startX, startY)).Count == MineCount;
            }
            //Phase 1
            {
                bool changed = false;
                do
                {
                    changed = false;
                    var squares = Active.ToList();
                    foreach (var s in squares)
                    {
                        changed = changed || TestSquare(s);
                    }
                } while (changed);
            }
            //Phase 2
            foreach (var pos in Active)
            {
                var reg = new MineRegion(Numbers[pos.x, pos.y], AdjUnknownPos(pos).ToArray());
                AddRegion(reg);
            }
            List<MineRegion> newRegs = new List<MineRegion>();
            do
            {
                newRegs.Clear();
                foreach (var reg in Regions)
                {
                    foreach (var otherReg in Regions)
                    {
                        if (reg == otherReg) continue;
                        if (reg.Contains(otherReg))
                        {
                            var newReg = new MineRegion(reg.Mines - otherReg.Mines,
                                reg.Positions.Except(otherReg.Positions).ToArray());
                            if (newReg.Mines == 0)
                            {
                                newReg.Positions.Foreach(ClearSquare);
                            }
                            if (newReg.Positions.Length == 1)
                            {
                                SetMine(newReg.Positions[0]);
                                Ext.Assert(newReg.Mines == 1);
                            }
                            else
                            {
                                newRegs.Add(newReg);
                            }
                        }
                    }
                }
                newRegs.ForEach(i => Regions.Add(i));
            }
            while (newRegs.Any());

            return Active.Count == 0;
        }
        private bool TestSquare((int x, int y) pos)
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
        private void ClearSquare((int x, int y) pos) => ClearSquare(pos.x, pos.y);
        private void ClearSquare(int x, int y)
        {
            Active.Add((x, y));
            if (!ClearSquares[x, y])
            {
                ClearSquares[x, y] = true;
                if (UnsolvedField.ClickSquare(x, y) == 0)
                {
                    AdjPos((x, y)).Where(i => !ClearSquares[i.Item1, i.Item2]).Foreach(i => ClearSquare(i.Item1, i.Item2));
                    Active.Remove((x, y));
                }
                if (Numbers[x, y] == 9) throw new Exception();
                AdjPos((x, y)).ForEach(CheckActive);
                CheckActive((x, y));
            }
        }
        private void CheckActive((int x, int y) pos)
        {
            if (AdjPos(pos).TrueForAll((i) =>
             Mines[i.Item1, i.Item2] || ClearSquares[i.Item1, i.Item2]))
            {
                if (Active.Contains(pos))
                {
                    Active.Remove(pos);
                }
            }
        }
        private void SetMine((int x, int y) pos)
        {
            Mines[pos.x, pos.y] = true;
            CheckActive(pos);
            AdjPos(pos).ForEach(CheckActive);
        }
        private List<(int, int)> AdjUnknownPos((int x, int y) pos)
                 => AdjPos(pos).Where(i => !ClearSquares[i.Item1, i.Item2]).ToList();
        private List<(int, int)> AdjCertainMines((int x, int y) pos)
             => AdjPos(pos).Where(i => Mines[i.Item1, i.Item2]).ToList();
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
        private void AddRegion(MineRegion region)
        {
            Regions.Add(region);
            foreach (var pos in RelevantSquares(region))
            {
                RegionsOnSquare[pos.Item1, pos.Item2].Add(region);
            }
        }
        private List<(int, int)> RelevantSquares(MineRegion region)
        {
            IEnumerable<(int, int)> res = AdjPos(region.Positions[0]);
            for (int i = 1; i < region.Positions.Length; i++)
            {
                res = res.Intersect(AdjPos(region.Positions[i]));
            }
            return res.ToList();
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