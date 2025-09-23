using System.Collections;
namespace Minesweeper
{
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