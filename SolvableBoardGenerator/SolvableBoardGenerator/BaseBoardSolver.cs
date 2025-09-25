using System.Reflection;

namespace Minesweeper
{
    /*
     * Pattern description:
     * PS1 pattern of size 1, meaning all mines were flagged around a number or it had only it's number as neighbors.
     * PS2 pattern of size 2, meaning 2 numbers were used to logicaly reason about a square.
     * PS3 dito with 3
     * PSN dito with N
     * PBF pattern discovered by brute forcing all possible mine positions. (all patterns can be classified as PBF)
     * PMC mine counting pattern
    */
    public abstract class BaseBoardSolver : IBoardSolver
    {
        protected IBoard Board;
        protected uint MineCount;
        protected bool[,] DiscoveredMines;
        protected sbyte[,] DiscoveredNumbers;
        protected const sbyte UndiscoveredNumber = -1;
        protected HashSet<(int x, int y)> ActiveNumbers = new HashSet<(int x, int y)>();
        protected HashSet<(int x, int y)> ActiveUnopenedSquares = new HashSet<(int x, int y)>();
        protected bool VerboseLogging = false;
        protected bool VerboseLoggingDuringPhasesForceDisable = false;
        public uint SquaresCleared { get; private set; }
        public double PercentageCleared => ((double)SquaresCleared) / Board.SizeX / Board.SizeY * 100.0;
        public BaseBoardSolver()
        {
            SquaresCleared = 0;
            Board = Minesweeper.Board.GetEmptyBoard();
            DiscoveredMines = new bool[0, 0];
            DiscoveredNumbers = new sbyte[0, 0];
            ResetSolver();
        }
        public BaseBoardSolver(IBoard board, bool verboseLogging = false)
        {
            if (board == null)
            {
                throw new ArgumentNullException($"{nameof(board)} was null in the constructor of {nameof(BaseBoardSolver)}, please call it with a non null {typeof(Board)}");
            }
            ChangeBoard(board);
            VerboseLogging = verboseLogging;
        }
        public void DisableAutoLoggingDuringPhases() => VerboseLoggingDuringPhasesForceDisable = true;
        public void ChangeBoard(IBoard board)
        {
            Board = board;
            MineCount = board.Mines;
            DiscoveredMines = new bool[board.SizeX, board.SizeY];
            DiscoveredNumbers = new sbyte[board.SizeX, board.SizeY];
            for (int x = 0; x < board.SizeX; x++)
            {
                for (int y = 0; y < board.SizeY; y++)
                {
                    DiscoveredNumbers[x, y] = UndiscoveredNumber;
                }
            }
            ResetSolver();
        }
        public void SetVerboseLogging(bool verboseLogging)
        {
            VerboseLogging = verboseLogging;
        }
        public virtual SolvabilityClass GetSolvabilityClass => SolvabilityClass.Unknown;
        public virtual bool IsSolvable((int x, int y) startPos) => IsSolvable(startPos.x, startPos.y);
        public virtual bool IsSolvable(int startX, int startY)
        {
            ResetSolver();
            try
            {
                DiscoveredNumbers[startX, startY] = Board.ClickOnSquare(startX, startY);
            }
            catch (MineExplosionException mee)
            {
                return false;
            }
            ActiveNumbers.Add((startX, startY));
            Board.GetNeighbors(startX, startY).Foreach(i => ActiveUnopenedSquares.Add(i));
            while (!Board.IsCleared())
            {
                bool progress = false;
                if (VerboseLogging && !VerboseLoggingDuringPhasesForceDisable)
                {
                    PrintCurrentStateBoard();
                }
                foreach (var f in PhaseSequence())
                {
                    if (f())
                    {
                        progress = true;
                        break;
                    }
                }
                if (!progress) { break; }
            }
            if (VerboseLogging)
            {
                Console.WriteLine($"Last state of board (cleared {PercentageCleared}%):");
                PrintCurrentStateBoard();
            }
            return Board.IsCleared();
        }
        protected virtual IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
        }
        protected bool TestPS1()
        {
            uint informationFound = 0;
            foreach (var pos in ActiveNumbers.ToList())
            {
                informationFound += TestSquare(pos) ? (uint)1 : 0;
            }
            if (informationFound > 0 && VerboseLogging)
            {
                Console.WriteLine($"{informationFound} PS1s found, current mine count: {MineCount}");
            }
            return informationFound > 0;
        }
        protected void ResetSolver()
        {
            SquaresCleared = 0;
            Board.ResetBoard();
            MineCount = Board.Mines;
            for (int x = 0; x < Board.SizeX; x++)
            {
                for (int y = 0; y < Board.SizeY; y++)
                {
                    DiscoveredMines[x, y] = false;
                    DiscoveredNumbers[x, y] = UndiscoveredNumber;
                }
            }
            ActiveNumbers = new HashSet<(int x, int y)>();
            ActiveUnopenedSquares = new HashSet<(int x, int y)>();
            foreach (var pos in Board.GetStartClears())
            {
                ClickSquare(pos);
            }
        }
        /// <summary>
        /// Tests Square for 1 sized pattern, returns true if it was a 1 sized pattern.
        /// </summary>
        protected bool TestSquare((int x, int y) pos)
        {
            var neighbors = Board.GetNeighbors(pos);
            var number = DiscoveredNumbers[pos.x, pos.y];
            var nonOpenedNeighbors = neighbors.Where(i => !IsOpenedSquare(i)).ToList();
            var setMinesNeighbors = neighbors.Where(i => IsSetMine(i)).ToList();
            if (nonOpenedNeighbors.Count() == number)
            {
                foreach (var s in nonOpenedNeighbors)
                {
                    SetMine(s);
                }
                ActiveNumbers.Remove(pos);
                return true;
            }
            if (setMinesNeighbors.Count() == number)
            {
                foreach (var s in nonOpenedNeighbors.Except(setMinesNeighbors))
                {
                    ClickSquare(s);
                }
                return true;
            }
            return false;
        }
        protected void SetMine(int x, int y)
        {
            if (!DiscoveredMines[x, y])
            {
                DiscoveredMines[x, y] = true;
                MineCount--;
                SquaresCleared++;
            }
            ActiveUnopenedSquares.Remove((x, y));
        }
        protected void SetMine((int x, int y) pos) => SetMine(pos.x, pos.y);
        protected void ClickSquare(int x, int y)
        {
            if (DiscoveredNumbers[x, y] != UndiscoveredNumber)
            {
                return;
            }
            DiscoveredNumbers[x, y] = Board.ClickOnSquare(x, y);
            ActiveNumbers.Add((x, y));
            ActiveUnopenedSquares.Remove((x, y));
            Board.GetNeighbors(x, y).Where(i => !IsOpenedSquare(i) && !IsSetMine(i)).Foreach(i => ActiveUnopenedSquares.Add(i));
            SquaresCleared++;
        }
        protected void ClickSquare((int x, int y) pos) => ClickSquare(pos.x, pos.y);
        protected bool IsOpenedSquare(int x, int y) => DiscoveredNumbers[x, y] != UndiscoveredNumber;
        protected bool IsOpenedSquare((int x, int y) pos) => IsOpenedSquare(pos.x, pos.y);
        protected bool IsSetMine(int x, int y) => DiscoveredMines[x, y];
        protected bool IsSetMine((int x, int y) pos) => IsSetMine(pos.x, pos.y);
        protected IEnumerable<(int x, int y)> GetUnopenedNeighbors((int x, int y) pos)
            => Board.GetNeighbors(pos).Where(i => !IsOpenedSquare(i));
        protected IEnumerable<(int x, int y)> GetOpenedNeighbors((int x, int y) pos)
            => Board.GetNeighbors(pos).Where(i => IsOpenedSquare(i));
        protected IEnumerable<(int x, int y)> GetConnectedUnopenSquares((int x, int y) pos1, (int x, int y) pos2)
            => GetUnopenedNeighbors(pos1).Intersect(GetUnopenedNeighbors(pos2));
        /// <summary>
        /// Intended for opened squares only.
        /// Connectivity returned is how many squares are shared between the two numbers.
        /// </summary>
        protected IEnumerable<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectedSquares)> ConnectedActiveNonMineNumbers((int x, int y) pos)
            => ConnectedNumbersWithConnectivity(pos).Where(i => ActiveNumbers.Contains(i.pos)).Select(i => (i.pos, (IReadOnlyList<(int x, int y)>)i.connectedSquares.Where(i => !IsSetMine(i)).ToList())).Where(i => i.Item2.Any());
        protected IEnumerable<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectedSquares)> ConnectedNumbersWithConnectivity((int x, int y) pos)
        {
            var neighbors = GetUnopenedNeighbors(pos).ToList();
            var neighborsOfNeighbors = neighbors.SelectMany(i => (GetOpenedNeighbors(i).CartesianProduct(i.AsSingleEnumerable()))).GroupBy(i => (i.Item1.x, i.Item1.y));
            foreach (var group in neighborsOfNeighbors)
            {
                if (group.Key == pos) { continue; }
                yield return (group.Key, group.Select(i => i.Item2).ToList());
            }
        }
        /// <summary>
        /// If called with an unopened Squares returns all opened numbers connected to that square,
        /// otherwise (called with an opened number) returns all numbers that have at least one unopened square, thats not marked as a mine, shared between them. 
        /// </summary>
        protected IEnumerable<(int x, int y)> ConnectedNumbers((int x, int y) pos)
        {
            if (IsOpenedSquare(pos))
            {
                foreach (var unopenedSquare in Board.GetNeighbors(pos).Where(i => !IsOpenedSquare(i)))
                {
                    foreach (var number in ConnectedNumbers(unopenedSquare))
                    {
                        if (number == pos)
                        {
                            continue;
                        }
                        yield return number;
                    }
                }
            }
            else
            {
                foreach (var number in Board.GetNeighbors(pos).Where(i => IsOpenedSquare(i) && !IsSetMine(i)))
                {
                    yield return number;
                }
            }
        }
        protected IEnumerable<T> GetCycleElements<T>(List<(T mrpn, List<T> edges)> group)
        {
            var graph = new Dictionary<T, HashSet<T>>();
            var deg1nodes = new Queue<T>();
            foreach (var node in group)
            {
                graph[node.mrpn] = node.edges.ToHashSet();
                if (node.edges.Count == 1)
                {
                    deg1nodes.Enqueue(node.mrpn);
                }
            }
            while (deg1nodes.Any())
            {
                var element = deg1nodes.Dequeue();
                if (!graph.ContainsKey(element))
                {
                    continue;
                }
                foreach (var edge in graph[element].Where(i => graph.ContainsKey(i)))
                {
                    graph[edge].Remove(element);
                    if (graph[edge].Count == 1)
                    {
                        deg1nodes.Enqueue(edge);
                    }
                }
                graph.Remove(element);
            }
            return graph.Keys;
        }
        public void PrintCurrentStateBoard()
        {
            //const char UnopenedSquare = '\u25A2';
            //const char BlackFlag = '\u2690';
            const char Flag = 'F';
            for (int y = 0; y < Board.SizeY; y++)
            {
                string toOutput = "";
                ConsoleColor toOutputColor = ConsoleColor.Gray;
                for (int x = 0; x < Board.SizeX; x++)
                {
                    //string c = $"{UnopenedSquare}";
                    string c = $" ";
                    if (DiscoveredMines[x, y])
                    {
                        c = $"{Flag}";
                    }
                    else if (DiscoveredNumbers[x, y] != UndiscoveredNumber)
                    {
                        c = DiscoveredNumbers[x, y].ToString();
                        if (c == "0")
                        {
                            c = ".";
                        }
                    }
                    ConsoleColor backColor = ConsoleColor.DarkGray;
                    if (DiscoveredMines[x, y])
                    {
                        backColor = ConsoleColor.Red;
                    }
                    else if (DiscoveredNumbers[x, y] == UndiscoveredNumber)
                    {
                        backColor = ConsoleColor.Gray;
                    }
                    if (toOutput.Length == 0)
                    {
                        toOutput = c;
                        toOutputColor = backColor;
                    }
                    else if (toOutputColor != backColor)
                    {
                        Ext.ConsoleWriteColor(toOutput, toOutputColor);
                        toOutput = c;
                        toOutputColor = backColor;
                    }
                    else
                    {
                        toOutput += c;
                    }
                }
                Ext.ConsoleWriteColor(toOutput, toOutputColor);
                Console.WriteLine();
            }
            Console.WriteLine($"Current mine count: {MineCount}");
        }
    }
}