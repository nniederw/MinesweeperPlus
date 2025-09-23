using System.Collections;
namespace Minesweeper
{
    public abstract class EfficientMineRegionPermutationBase
    {
        protected IReadOnlyDictionary<(int x, int y), int> IndexLookupTable = new Dictionary<(int x, int y), int>();
        protected bool VerboseLogging = false;
        protected const uint StartOfProgressUpdates = 10000;
        protected const uint ProgressUpdateEvery = 10000;
        public IEnumerable<(int x, int y)> SquaresInPermutation => IndexLookupTable.Keys;
    }
    public class EfficientMineRegionPermutation : EfficientMineRegionPermutationBase
    {
        private IReadOnlyList<BitArray> Permutations;
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
            uint MaxNewPermutations = p1.PermutationCount * p2.PermutationCount;
            if (verboseLogging)
            {
                Console.WriteLine($"Intersecting two {nameof(EfficientMineRegionPermutation)}, with current valid permutations: {p1.PermutationCount} & {p1.PermutationCount}, giving a total of {MaxNewPermutations} possible permutations.");
            }
            var possiblePermutations = p1.AllPermutations().CartesianProduct(p2.AllPermutations());
            var firstPPerm = possiblePermutations.First();
            var intersectionSquares = firstPPerm.Item1.SharedPositions(firstPPerm.Item2).ToList();
            IEnumerable<(PermutationWrapper, PermutationWrapper)> validPermutations;
            if (verboseLogging && StartOfProgressUpdates <= p1.PermutationCount * p2.PermutationCount)
            {
                uint checkedPermutations = 0;
                uint validPermutationCount = 0;
                validPermutations = possiblePermutations.Where(i =>
                {
                    if (checkedPermutations % ProgressUpdateEvery == 0)
                    {
                        Console.WriteLine($"Checked a total of {checkedPermutations} ({((double)checkedPermutations) / MaxNewPermutations * 100.0}%), valid ones so far: {validPermutationCount} ({((double)validPermutationCount) / checkedPermutations * 100.0}%)");
                    }
                    checkedPermutations++;
                    if (i.Item1.Intersectable(i.Item2, intersectionSquares))
                    {
                        validPermutationCount++;
                        return true;
                    }
                    return false;
                });
            }
            else
            {
                validPermutations = possiblePermutations.Where(i => i.Item1.Intersectable(i.Item2, intersectionSquares));
            }
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
    }
}