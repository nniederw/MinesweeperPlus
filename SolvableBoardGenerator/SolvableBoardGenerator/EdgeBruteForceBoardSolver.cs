namespace Minesweeper
{
    public class EdgeBruteForceBoardSolver : BruteForceBoardSolver
    {
        public EdgeBruteForceBoardSolver() : base() { }
        public EdgeBruteForceBoardSolver(IBoard board, bool verboseLogging = false) : base(board, verboseLogging) { }
        //public override IBoardSolver Construct(IBoard board, bool verboseLogging = false) => new EdgeBruteForceBoardSolver(board, verboseLogging);
        public override SolvabilityClass GetSolvabilityClass => SolvabilityClass.Complete;
        protected override IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
            yield return TestEdgePBF;
        }
        protected bool TestEdgePBF()
        {
            if (VerboseLogging)
            {
                Console.WriteLine($"Didn't find a PS1. Moving on to PBF.");
            }
            var unopenedSquares = Board.AllSquares().Where(i => !IsOpenedSquare(i) && !IsSetMine(i)).ToList();
            var relevantNumbers = unopenedSquares.SelectMany(i => Board.GetNeighbors(i).Prepend(i)).Where(i => IsOpenedSquare(i)).Distinct().ToList();
            var determinableSquares = EdgeBruteforceSquaresAlgo(unopenedSquares, relevantNumbers, MineCount).ToList();
            if (VerboseLogging)
            {
                Console.WriteLine($"Found {determinableSquares.Count} many squares information with brute forcing.");
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
        /// This function may also just give minecount info instead of whole determinable squares.
        /// </summary>
        private List<((int x, int y) pos, bool mine)> EdgeBruteforceSquaresAlgo(List<(int x, int y)> unopenedSquares, List<(int x, int y)> relevantNumbers, uint mineCount)
        {
            //todo, consider that 1 2 are possible, 3 not, 4 possible again. aka check ever number between 1 & MineCount.
            bool?[,] result = new bool?[Board.SizeX, Board.SizeY];
            var relevantNumbHash = relevantNumbers.ToHashSet();
            var relevantUnopSquaresHash = relevantNumbHash.SelectMany(i => Board.GetNeighbors(i)).Where(i => !IsOpenedSquare(i) && !IsSetMine(i)).ToHashSet();
            var relevantUnopSquares = relevantUnopSquaresHash.ToList();
            //var relevantUnopSquares = unopenedSquares.Where(relavantNumbHash.Contains).ToList();
            var restUnopSquares = unopenedSquares.Where(i => !relevantUnopSquaresHash.Contains(i)).ToList();
            uint maxMinesInEdge = Math.Min(mineCount, (uint)relevantUnopSquares.Count);
            uint minMinesInEdge = (uint)Math.Max(0, mineCount - restUnopSquares.Count);
            IEnumerable<List<(int x, int y)>>[] enumerables = new IEnumerable<List<(int x, int y)>>[maxMinesInEdge + 1]; //index i stands for i + 1 mines
            bool[] anyValidPerm = new bool[maxMinesInEdge + 1]; //index i stands for i mines
            for (uint i = 0; i <= maxMinesInEdge; i++)
            {
                if (i < minMinesInEdge)
                {
                    anyValidPerm[i] = false;
                    enumerables[i] = Enumerable.Empty<List<(int x, int y)>>();
                    continue;
                }
                var perm = Combinatorics.GetCombinationsIterative(relevantUnopSquares, i).Where(i => ValidPermutation(relevantNumbers, i));
                enumerables[i] = new PartiallyMaterializedEnumerable<List<(int x, int y)>>(perm, 1).GetEnumerable();
                anyValidPerm[i] = enumerables[i].Any();
            }
            if (restUnopSquares.Any() && anyValidPerm.Count(i => i) == 1)
            {
                uint EdgeMineCount = 0; //Note: edge mine count without already set mines
                for (uint i = 0; i <= maxMinesInEdge; i++)
                {
                    if (anyValidPerm[i])
                    {
                        EdgeMineCount = i;
                        break;
                    }
                }
                if (EdgeMineCount == mineCount)
                {
                    return restUnopSquares.Select(i => (i, false)).ToList();
                }
                return BruteForceSquares(relevantUnopSquares, relevantNumbers, EdgeMineCount); //Todo possible optimisations from having 1 valid permutation already computed.
            }
            bool firstTime = true;
            foreach (var enumerable in enumerables.Where(i => i.Any()))
            {
                foreach (var permutation in enumerable)
                {
                    var permuteHash = permutation.ToHashSet();
                    if (firstTime)
                    {
                        foreach (var pos in relevantUnopSquares)
                        {
                            result[pos.x, pos.y] = permuteHash.Contains(pos);
                        }
                        firstTime = false;
                        continue;
                    }
                    foreach (var pos in relevantUnopSquares)
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
            }
            List<((int x, int y) pos, bool mine)> res = new();
            foreach (var pos in relevantUnopSquares)
            {
                if (result[pos.x, pos.y] != null)
                {
                    res.Add((pos, result[pos.x, pos.y].Value));
                }
            }
            return res;
        }
    }
}