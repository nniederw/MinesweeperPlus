namespace Minesweeper
{
    public class SimpleBoardSolver
    {
        private Board Board;
        private uint MineCount;
        private bool[,] DiscoveredMines;
        private sbyte[,] DiscoveredNumbers;
        private const sbyte UndiscoveredNumber = -1;
        HashSet<(int x, int y)> ActiveNumbers = new HashSet<(int x, int y)>();
        HashSet<(int x, int y)> ActiveUnopenedSquares = new HashSet<(int x, int y)>();
        public SimpleBoardSolver(Board board)
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
        }
        public bool IsSolvable(int startX, int startY)
        {
            /*
             * 1: 1 sized patterns                  
             * 2: try permutations
            */
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
            Board.GetNeighbors(startX, startY).ForEach(i => ActiveUnopenedSquares.Add(i));
            while (!Board.IsCleared())
            {
                //Phase 1
                if (!ActiveNumbers.ToList().TrueForAll(s => !TestSquare(s)))
                {
                    continue;
                }
                //Phase 2
                var unopenedSquares = Board.AllSquares().Where(i => !IsOpenedSquare(i) && !IsSetMine(i)).ToList();
                var relevantNumbers = unopenedSquares.SelectMany(i => Board.GetNeighbors(i).Prepend(i)).Where(i => IsOpenedSquare(i));
                var determinableSquares = BruteforceSquares(unopenedSquares, relevantNumbers.ToList());
                foreach (var square in determinableSquares)
                {
                    if (square.mine)
                    {
                        SetMine(square.pos);
                    }
                    else
                    {
                        ClickSquare(square.pos);
                    }
                }
                if (!determinableSquares.Any()) { break; }
            }
            return Board.IsCleared();
        }
        private void ResetSolver()
        {
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
        }
        /// <summary>
        /// Tests Square for 1 sized pattern, returns true if it was a 1 sized pattern.
        /// </summary>
        private bool TestSquare((int x, int y) pos)
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
        private void SetMine(int x, int y)
        {
            if (!DiscoveredMines[x, y])
            {
                DiscoveredMines[x, y] = true;
                MineCount--;
            }
            ActiveUnopenedSquares.Remove((x, y));
        }
        private void SetMine((int x, int y) pos) => SetMine(pos.x, pos.y);
        private void ClickSquare(int x, int y)
        {
            DiscoveredNumbers[x, y] = Board.ClickOnSquare(x, y);
            ActiveNumbers.Add((x, y));
            Board.GetNeighbors(x, y).Where(i => !IsOpenedSquare(i) && !IsSetMine(i)).Foreach(i => ActiveUnopenedSquares.Add(i));
        }
        private void ClickSquare((int x, int y) pos) => ClickSquare(pos.x, pos.y);
        private bool IsOpenedSquare(int x, int y) => DiscoveredNumbers[x, y] != UndiscoveredNumber;
        private bool IsOpenedSquare((int x, int y) pos) => IsOpenedSquare(pos.x, pos.y);
        private bool IsSetMine(int x, int y) => DiscoveredMines[x, y];
        private bool IsSetMine((int x, int y) pos) => IsSetMine(pos.x, pos.y);
        /// <summary>
        /// returns true if square is a guaranteed a mine, false if it is guaranteed not a mine, null if it can be either with the current information.
        /// </summary>
        private List<((int x, int y) pos, bool mine)> BruteforceSquares(List<(int x, int y)> unopenedSquares, List<(int x, int y)> relevantNumbers)
        {
            int validPermutationsTested = 0;
            bool?[,] result = new bool?[Board.SizeX, Board.SizeY];
            var validPermuts = Combinatorics.GetCombinationsIterative(unopenedSquares, MineCount).Where(i => ValidPermutation(relevantNumbers, i));
            bool firstTime = true;
            foreach (var permutation in validPermuts)
            {
                var permuteHash = permutation.ToHashSet();
                //Console.WriteLine($"permutationsTested: {permutationsTested}, validPermutationsTested: {validPermutationsTested}");
                validPermutationsTested++;
                if (firstTime)
                {
                    foreach (var pos in unopenedSquares)
                    {
                        result[pos.x, pos.y] = permuteHash.Contains(pos);
                    }
                    firstTime = false;
                    continue;
                }
                foreach (var pos in unopenedSquares)
                {
                    if (result[pos.x, pos.y] != null)
                    {
                        if (result[pos.x, pos.y] != permuteHash.Contains(pos))
                        {
                            result[pos.x, pos.y] = null;
                        }
                    }
                }
            }
            List<((int x, int y) pos, bool mine)> res = new();
            foreach (var pos in unopenedSquares)
            {
                if (result[pos.x, pos.y] != null)
                {
                    res.Add((pos, result[pos.x, pos.y].Value));
                }
            }
            return res;
        }
        private bool ValidPermutation(List<(int x, int y)> relevantNumbers, List<(int x, int y)> permutationMines)
        {
            var permutationMinesHashed = permutationMines.ToHashSet();
            foreach (var square in relevantNumbers)
            {
                var neighbors = Board.GetNeighbors(square);
                int number = DiscoveredNumbers[square.x, square.y];
                int setMines = neighbors.Count(IsSetMine);
                int neighborsPermMines = neighbors.Count(permutationMinesHashed.Contains);
                if (number != setMines + neighborsPermMines)
                {
                    return false;
                }
            }
            return true;
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