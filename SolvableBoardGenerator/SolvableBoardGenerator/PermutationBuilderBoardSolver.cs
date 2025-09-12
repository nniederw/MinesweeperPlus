using Minesweeper;
using System.Collections.Generic;
namespace Minesweeper
{
    public class PermutationBuilderBoardSolver : BaseBoardSolver
    {
        public PermutationBuilderBoardSolver(Board board, bool verboseLogging = false) : base(board, verboseLogging) { }
        public override IBoardSolver Construct(Board board, bool verboseLogging = false) => new PermutationBuilderBoardSolver(board, verboseLogging);
        protected override IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
            yield return PermutationBuildingAlgorithm;
        }
        private bool PermutationBuildingAlgorithm()
        {
            if (!ActiveNumbers.Any())
            {
                return false;
            }
            var unusedNumbers = ActiveNumbers.ToHashSet();
            var usedNumbers = new HashSet<(int x, int y)>();
            uint separateLogic = 0;
            while (unusedNumbers.Any())
            {
                separateLogic++;
                uint numbersInCurrentLogic = 0;
                var possibleContinuations = new HashSet<(int x, int y)>();
                possibleContinuations.Add(unusedNumbers.First());
                MineRegionPermutation? CurrentPermutationGroup = null;
                while (possibleContinuations.Any())
                {
                    var continuation = possibleContinuations.First();
                    possibleContinuations.Remove(continuation);
                    var region = MineRegionPermutationFromNumber(continuation);
                    usedNumbers.Add(continuation);
                    unusedNumbers.Remove(continuation);
                    numbersInCurrentLogic++;
                    var connectedNumbers = ConnectedNumbers(continuation).Where(i => unusedNumbers.Contains(i) && !usedNumbers.Contains(i)).ToList();
                    foreach (var cNumber in connectedNumbers)
                    {
                        possibleContinuations.Add(cNumber);
                    }
                    if (CurrentPermutationGroup == null)
                    {
                        CurrentPermutationGroup = region;
                    }
                    else
                    {
                        CurrentPermutationGroup = CurrentPermutationGroup.Intersection(region);
                    }
                    bool foundNewInfo = false;
                    foreach (var info in CurrentPermutationGroup.GetInformation())
                    {
                        if (info.mine)
                        {
                            if (!IsSetMine(info.pos))
                            {
                                foundNewInfo = true;
                                SetMine(info.pos);
                            }
                        }
                        else
                        {
                            foundNewInfo = true;
                            ClickSquare(info.pos);
                        }
                    }
                    if (foundNewInfo)
                    {
                        if (VerboseLogging)
                        {
                            Console.WriteLine($"Current logic had new information, breaking early. Had a total of {numbersInCurrentLogic} numbers in logic.");
                        }
                        return true;
                    }
                }
                if (VerboseLogging)
                {
                    Console.WriteLine($"Finnished a total of {separateLogic} separable logics, current one finished had a total of {numbersInCurrentLogic} numbers");
                }
            }
            return false;
        }
        private MineRegionPermutation MineRegionPermutationFromNumber((int x, int y) pos)
        {
            if (DiscoveredNumbers[pos.x, pos.y] == UndiscoveredNumber)
            {
                throw new Exception($"Called {nameof(MineRegionPermutationFromNumber)} in {nameof(PermutationBuilderBoardSolver)} with a unopened square, which isn't allowed.");
            }
            var totalNeighbors = Board.GetNeighbors(pos).ToList();
            uint mines = (uint)DiscoveredNumbers[pos.x, pos.y] - (uint)totalNeighbors.Count(i => IsSetMine(i));
            var neighbors = totalNeighbors.Where(i => !IsSetMine(i) && !IsOpenedSquare(i)).ToList();
            uint neighborsCount = (uint)neighbors.Count;
            var boolPermuts = Combinatorics.GetCombinationsIterative(mines, neighborsCount);
            var permuts = boolPermuts.Select(i => neighbors.Zip(i).ToList());
            return new MineRegionPermutation(permuts, VerboseLogging);

        }
        private class MineRegionPermutation
        {
            private List<Dictionary<(int x, int y), bool>> Permutations;
            private bool VerboseLogging = false;
            public MineRegionPermutation(IEnumerable<List<((int x, int y) pos, bool mine)>> ValidPermutations, bool verboseLogging = false)
            {
                Permutations = new List<Dictionary<(int x, int y), bool>>();
                ValidPermutations.Foreach(p =>
                    Permutations.Add(
                        new Dictionary<(int x, int y), bool>(
                            p.Select(i => new KeyValuePair<(int x, int y), bool>(i.pos, i.mine))))
                );
                VerboseLogging = verboseLogging;
            }
            /// <summary>
            /// Note: Dictionaries aren't copied, don't modify them later.
            /// </summary>
            public MineRegionPermutation(IEnumerable<Dictionary<(int x, int y), bool>> permutations, bool verboseLogging = false)
            {
                Permutations = permutations.ToList();
                VerboseLogging = verboseLogging;
            }
            public IEnumerable<(int x, int y)> Squares => Permutations.First().Keys;
            public MineRegionPermutation Intersection(MineRegionPermutation other)
                => Intersection(this, other);
            public static MineRegionPermutation Intersection(MineRegionPermutation p1, MineRegionPermutation p2)
            {
                bool verboseLogging = p1.VerboseLogging || p2.VerboseLogging;
                if (verboseLogging)
                {
                    var p1C = p1.Squares.Count();
                    var p2C = p2.Squares.Count();
                    Console.WriteLine($"Intersecting two {nameof(MineRegionPermutation)}, with current valid permutations: {p1C} & {p2C}, giving a total of {p1C * p2C} possible permutations.");
                }
                var intersectionSquares = p1.Squares.Intersect(p2.Squares).ToList();
                var possiblePermutations = p1.Permutations.CartesianProduct(p2.Permutations);
                var validPermutations = possiblePermutations.Where(i => ValidIntersection(i.Item1, i.Item2, intersectionSquares));
                var combinedPermutations = validPermutations.Select(i => CombinePermutation(i.Item1, i.Item2));
                var res = new MineRegionPermutation(combinedPermutations, verboseLogging);
                if (verboseLogging)
                {
                    var pC = p1.Squares.Count() * p2.Squares.Count();
                    Console.WriteLine($"Got a total of {res.Squares.Count()} valid permutations, from the total possible {pC}.");
                }
                return res;
            }
            public IEnumerable<((int x, int y) pos, bool mine)> GetInformation()
            {
                if (!Permutations.Any())
                {
                    return Enumerable.Empty<((int x, int y) pos, bool mine)>();
                }
                Dictionary<(int x, int y), bool> result = new Dictionary<(int x, int y), bool>();
                foreach (var kvp in Permutations[0])
                {
                    result.Add(kvp.Key, kvp.Value);
                }
                for (int i = 1; i < Permutations.Count; i++)
                {
                    foreach (var kvp in Permutations[i])
                    {
                        if (result.ContainsKey(kvp.Key) && result[kvp.Key] != kvp.Value)
                        {
                            result.Remove(kvp.Key);
                        }
                    }
                }
                return result.Select(i => (i.Key, i.Value));
            }
            private static bool ValidIntersection(Dictionary<(int x, int y), bool> p1, Dictionary<(int x, int y), bool> p2, List<(int x, int y)> intersectionSquares)
                => intersectionSquares.TrueForAll(pos => p1[pos] == p2[pos]);
            private static Dictionary<(int x, int y), bool> CombinePermutation(Dictionary<(int x, int y), bool> p1, Dictionary<(int x, int y), bool> p2)
            {
                var res = new Dictionary<(int x, int y), bool>(p1);
                foreach (var kvp in p2)
                {
                    res[kvp.Key] = kvp.Value;
                }
                return res;
            }
        }
    }
}