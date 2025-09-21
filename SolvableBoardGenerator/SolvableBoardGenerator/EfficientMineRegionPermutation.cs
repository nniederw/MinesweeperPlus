using System.Collections;
namespace Minesweeper
{
    public class EfficientMineRegionPermutation
    {
        private IReadOnlyDictionary<(int x, int y), int> IndexLookupTable;
        private IReadOnlyList<BitArray> Permutations;
        private bool VerboseLogging = false;
        public EfficientMineRegionPermutation(uint mines, IEnumerable<(int x, int y)> region_, bool verboseLogging = false)
        {
            var region = region_.ToList();
            uint squaresCount = (uint)region.Count;
            var boolPermuts = Combinatorics.GetCombinationsIterative(mines, squaresCount);
            Permutations = boolPermuts.Select(i => new BitArray(i)).ToList();
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
        public EfficientMineRegionPermutation(IEnumerable<BitArray> validPermutations, IReadOnlyDictionary<(int x, int y), int> indexLookupTable, bool verboseLogging = false)
        {
            Permutations = validPermutations.ToList();
            IndexLookupTable = indexLookupTable;
            VerboseLogging = verboseLogging;
        }
        public IEnumerable<(int x, int y)> Numbers => IndexLookupTable.Keys;
        public IEnumerable<PermutationWrapper> AllPermutations()
        {
            foreach (var perm in Permutations)
            {
                yield return new PermutationWrapper(perm, IndexLookupTable);
            }
        }
        public uint PermutationCount => (uint)Permutations.Count;
        public EfficientMineRegionPermutation Intersection(EfficientMineRegionPermutation other)
            => Intersection(this, other);
        public static EfficientMineRegionPermutation Intersection(EfficientMineRegionPermutation p1, EfficientMineRegionPermutation p2)
        {
            bool verboseLogging = p1.VerboseLogging || p2.VerboseLogging;
            if (verboseLogging)
            {
                var p1C = p1.PermutationCount;
                var p2C = p2.PermutationCount;
                Console.WriteLine($"Intersecting two {nameof(EfficientMineRegionPermutation)}, with current valid permutations: {p1C} & {p2C}, giving a total of {p1C * p2C} possible permutations.");
            }
            var possiblePermutations = p1.AllPermutations().CartesianProduct(p2.AllPermutations());
            var firstPPerm = possiblePermutations.First();
            var intersectionSquares = firstPPerm.Item1.SharedPositions(firstPPerm.Item2).ToList();
            var validPermutations = possiblePermutations.Where(i => i.Item1.Intersectable(i.Item2, intersectionSquares));
            var combinedLookupTable = firstPPerm.Item1.CombineLookupTables(firstPPerm.Item2.IndexLookupTable);
            var combinedPermutations = validPermutations.Select(i => i.Item1.Intersect(i.Item2, combinedLookupTable)).Select(i => i.Permutation);
            var res = new EfficientMineRegionPermutation(combinedPermutations, combinedLookupTable, verboseLogging);
            if (verboseLogging)
            {
                Console.WriteLine($"Got a total of {res.PermutationCount} valid permutations.");
            }
            return res;
        }
        public IEnumerable<((int x, int y) pos, bool mine)> GetInformation()
        {
            var resultMines = (BitArray)Permutations.First().Clone(); //stays 1 if all permutations have a 1 in them
            var resultNonMines = (BitArray)resultMines.Clone(); //stays 0 if all permutations have a 0 in them
            for (int i = 1; i < PermutationCount; i++)
            {
                resultMines.And(Permutations[i]);
                resultNonMines.Or(Permutations[i]);
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
        public class PermutationWrapper
        {
            public BitArray Permutation;
            public IReadOnlyDictionary<(int x, int y), int> IndexLookupTable;
            public PermutationWrapper(BitArray permutation, IReadOnlyDictionary<(int x, int y), int> indexLookupTable)
            {
                Permutation = permutation;
                IndexLookupTable = indexLookupTable;
            }
            public bool At((int x, int y) pos) => Permutation[IndexLookupTable[pos]];
            public IEnumerable<(int x, int y)> SharedPositions(PermutationWrapper other)
                => IndexLookupTable.KeyIntersection(other.IndexLookupTable);
            public bool Intersectable(PermutationWrapper other, IEnumerable<(int x, int y)> sharedPositions)
            {
                foreach (var pos in sharedPositions)
                {
                    if (At(pos) != other.At(pos))
                    {
                        return false;
                    }
                }
                return true;
            }
            public Dictionary<(int x, int y), int> CombineLookupTables(IReadOnlyDictionary<(int x, int y), int> otherIndexLookupTable)
            {
                Dictionary<(int x, int y), int> result = new Dictionary<(int x, int y), int>();
                foreach (var kvp in IndexLookupTable)
                {
                    result[kvp.Key] = kvp.Value;
                }
                int currentNewIndex = IndexLookupTable.Count;
                foreach (var kvp in otherIndexLookupTable)
                {
                    if (!result.ContainsKey(kvp.Key))
                    {
                        result.Add(kvp.Key, currentNewIndex);
                        currentNewIndex++;
                    }
                }
                return result;
            }
            public PermutationWrapper Intersect(PermutationWrapper other, IReadOnlyDictionary<(int x, int y), int> newIndexLookupTable)
            {
                int newLength = newIndexLookupTable.Count;
                BitArray NewBArr = new BitArray(newLength);
                foreach (var kvp in newIndexLookupTable)
                {
                    if (IndexLookupTable.ContainsKey(kvp.Key))
                    {
                        NewBArr.Set(kvp.Value, Permutation[IndexLookupTable[kvp.Key]]);
                        continue;
                    }
                    NewBArr.Set(kvp.Value, other.Permutation[other.IndexLookupTable[kvp.Key]]);
                }
                return new PermutationWrapper(NewBArr, newIndexLookupTable);
            }
        }
    }
}