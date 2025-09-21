namespace Minesweeper
{
    public class BruteForceBoardSolver : BaseBoardSolver
    {
        public BruteForceBoardSolver() : base() { }
        public BruteForceBoardSolver(IBoard board, bool verboseLogging = false) : base(board, verboseLogging) { }
        //public override IBoardSolver Construct(IBoard board, bool verboseLogging = false) => new BruteForceBoardSolver(board, verboseLogging);
        public override SolvabilityClass GetSolvabilityClass => SolvabilityClass.Complete;
        protected override IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
            yield return TestPBF;
        }
        protected bool TestPBF()
        {
            if (VerboseLogging)
            {
                Console.WriteLine($"Didn't find a PS1. Moving on to PBF.");
            }
            var unopenedSquares = Board.AllSquares().Where(i => !IsOpenedSquare(i) && !IsSetMine(i)).ToList();
            var relevantNumbers = unopenedSquares.SelectMany(i => Board.GetNeighbors(i).Prepend(i)).Where(i => IsOpenedSquare(i)).Distinct().ToList();
            var determinableSquares = BruteForceSquares(unopenedSquares, relevantNumbers, MineCount).ToList();
            if (VerboseLogging)
            {
                Console.WriteLine($"Found {determinableSquares.Count} many squares information with PBF.");
            }
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
            return determinableSquares.Any();
        }
        /// <summary>
        /// returns true if square is a guaranteed a mine, false if it is guaranteed not a mine.
        /// </summary>
        protected List<((int x, int y) pos, bool mine)> BruteForceSquares(List<(int x, int y)> unopenedSquares, List<(int x, int y)> relevantNumbers, uint mineCount)
        {
            int validPermutationsTested = 0;
            bool?[,] result = new bool?[Board.SizeX, Board.SizeY];
            var validPermuts = Combinatorics.GetCombinationsIterative(unopenedSquares, mineCount).Where(i => ValidPermutation(relevantNumbers, i));
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
        protected bool ValidPermutation(List<(int x, int y)> relevantNumbers, List<(int x, int y)> permutationMines)
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
    }
}