using System.Collections;
namespace Minesweeper
{
    public class CountedEfficientMineRegionPermutation : EfficientMineRegionPermutationBase
    {
        private IReadOnlyList<(BitArray permutation, uint mineCount)> Permutations;
        public uint MinMineCount { get; private set; }
        public uint MaxMineCount { get; private set; }
        public CountedEfficientMineRegionPermutation(uint mines, IEnumerable<(int x, int y)> region_, bool verboseLogging = false)
        {
            Permutations = new List<(BitArray permutation, uint mineCount)>(); //to shut the compiler up
            Initialize(mines, region_, verboseLogging);
            CountMinMaxMineCount();
        }
        public CountedEfficientMineRegionPermutation(uint mines, IEnumerable<(int x, int y)> region_, uint minMineCount, uint maxMineCount, bool verboseLogging = false)
        {
            Permutations = new List<(BitArray permutation, uint mineCount)>(); //to shut the compiler up
            Initialize(mines, region_, verboseLogging);
            MinMineCount = minMineCount;
            MaxMineCount = maxMineCount;
        }
        public CountedEfficientMineRegionPermutation(IEnumerable<(BitArray permutation, uint mineCount)> permutations, IReadOnlyDictionary<(int x, int y), int> indexLookupTable, bool verboseLogging = false)
        {
            Permutations = permutations.ToList();
            IndexLookupTable = indexLookupTable;
            VerboseLogging = verboseLogging;
            CountMinMaxMineCount();
        }
        ///Note, Permutations are just assigned to permutations, no copy is made
        public CountedEfficientMineRegionPermutation(IReadOnlyList<(BitArray permutation, uint mineCount)> permutations, IReadOnlyDictionary<(int x, int y), int> indexLookupTable, uint minMineCount, uint maxMineCount, bool verboseLogging = false)
        {
            Permutations = permutations;
            IndexLookupTable = indexLookupTable;
            VerboseLogging = verboseLogging;
            MinMineCount = minMineCount;
            MaxMineCount = maxMineCount;
        }
        private void Initialize(uint mines, IEnumerable<(int x, int y)> region_, bool verboseLogging = false)
        {
            var region = region_.ToList();
            uint squaresCount = (uint)region.Count;
            var boolPermuts = Combinatorics.GetCombinationsIterative(mines, squaresCount);
            Permutations = boolPermuts.Select(i => (new BitArray(i), mines)).ToList();
            VerboseLogging = verboseLogging;
            int index = 0;
            Dictionary<(int x, int y), int> lookupTable = new Dictionary<(int x, int y), int>();
            foreach (var pos in region)
            {
                lookupTable.Add(pos, index);
                index++;
            }
            IndexLookupTable = lookupTable;
        }
        private void CountMinMaxMineCount()
        {
            MinMineCount = uint.MaxValue;
            MaxMineCount = 0;
            foreach (var perm in Permutations)
            {
                MinMineCount = Math.Min(MinMineCount, perm.mineCount);
                MaxMineCount = Math.Max(MaxMineCount, perm.mineCount);
            }
        }
        public IEnumerable<CountedPermutationWrapper> AllPermutations()
        {
            foreach (var perm in Permutations)
            {
                yield return new CountedPermutationWrapper(perm.permutation, perm.mineCount, IndexLookupTable);
            }
        }
        public uint PermutationCount => (uint)Permutations.Count;
        public CountedEfficientMineRegionPermutation Intersection(CountedEfficientMineRegionPermutation other)
            => Intersection(this, other);
        public static CountedEfficientMineRegionPermutation Intersection(CountedEfficientMineRegionPermutation p1, CountedEfficientMineRegionPermutation p2)
        {
            bool verboseLogging = p1.VerboseLogging || p2.VerboseLogging;
            uint MaxNewPermutations = p1.PermutationCount * p2.PermutationCount;
            if (verboseLogging)
            {
                Console.WriteLine($"Intersecting two {nameof(CountedEfficientMineRegionPermutation)}, with current valid permutations: {p1.PermutationCount} & {p1.PermutationCount}, giving a total of {MaxNewPermutations} possible permutations.");
            }
            var possiblePermutations = p1.AllPermutations().CartesianProduct(p2.AllPermutations());
            var firstPPerm = possiblePermutations.First();
            var intersectionSquares = firstPPerm.Item1.SharedPositions(firstPPerm.Item2).ToList();
            IEnumerable<(CountedPermutationWrapper, CountedPermutationWrapper)> validPermutations;
            validPermutations = possiblePermutations.Where(i => i.Item1.Intersectable(i.Item2, intersectionSquares));
            var combinedLookupTable = firstPPerm.Item1.CombineLookupTables(firstPPerm.Item2.IndexLookupTable);
            var combinedPermutations = validPermutations.Select(i => i.Item1.Intersect(i.Item2, combinedLookupTable, intersectionSquares)).Select(i => (i.Permutation, i.MineCount));
            uint minMineCount = uint.MaxValue;
            uint maxMineCount = 0;
            var builtPermutations = combinedPermutations.Foreach(i =>
            {
                minMineCount = Math.Min(minMineCount, i.MineCount);
                maxMineCount = Math.Max(maxMineCount, i.MineCount);
            }).ToList();
            var res = new CountedEfficientMineRegionPermutation(builtPermutations, combinedLookupTable, minMineCount, maxMineCount, verboseLogging);
            if (verboseLogging)
            {
                Console.WriteLine($"Got a total of {res.PermutationCount} valid permutations.");
            }
            return res;
        }
        public IEnumerable<((int x, int y) pos, bool mine)> GetInformation()
        {
            var resultMines = (BitArray)Permutations.First().permutation.Clone(); //stays 1 if all permutations have a 1 in them
            var resultNonMines = (BitArray)resultMines.Clone(); //stays 0 if all permutations have a 0 in them
            for (int i = 1; i < PermutationCount; i++)
            {
                resultMines.And(Permutations[i].permutation);
                resultNonMines.Or(Permutations[i].permutation);
            }
            List<((int x, int y) pos, bool mine)> result = new List<((int x, int y) pos, bool mine)>();
            foreach (var kvp in IndexLookupTable)
            {
                int index = kvp.Value;
                if (resultMines[index])
                {
                    result.Add((kvp.Key, true));
                    continue;
                }
                if (!resultNonMines[index])
                {
                    result.Add((kvp.Key, false));
                }
            }
            return result;
        }
    }
}