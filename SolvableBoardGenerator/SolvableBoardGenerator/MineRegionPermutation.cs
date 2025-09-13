namespace Minesweeper
{
    public class MineRegionPermutation
    {
        private IReadOnlyList<Dictionary<(int x, int y), bool>> Permutations;
        //private Dictionary<(int x, int y), bool> IndexLookupTable;
        private bool VerboseLogging = false;
        public MineRegionPermutation(IEnumerable<List<((int x, int y) pos, bool mine)>> ValidPermutations, bool verboseLogging = false)
        {
            var permuts = new List<Dictionary<(int x, int y), bool>>();
            ValidPermutations.Foreach(p =>
                permuts.Add(
                    new Dictionary<(int x, int y), bool>(
                        p.Select(i => new KeyValuePair<(int x, int y), bool>(i.pos, i.mine))))
            );
            Permutations = permuts;
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
        public uint PermutationCount => (uint)Permutations.Count;
        public MineRegionPermutation Intersection(MineRegionPermutation other)
            => Intersection(this, other);
        public static MineRegionPermutation Intersection(MineRegionPermutation p1, MineRegionPermutation p2)
        {
            bool verboseLogging = p1.VerboseLogging || p2.VerboseLogging;
            if (verboseLogging)
            {
                var p1C = p1.Permutations.Count();
                var p2C = p2.Permutations.Count();
                Console.WriteLine($"Intersecting two {nameof(MineRegionPermutation)}, with current valid permutations: {p1C} & {p2C}, giving a total of {p1C * p2C} possible permutations.");
            }
            var intersectionSquares = p1.Squares.Intersect(p2.Squares).ToList();
            var possiblePermutations = p1.Permutations.CartesianProduct(p2.Permutations);
            var validPermutations = possiblePermutations.Where(i => ValidIntersection(i.Item1, i.Item2, intersectionSquares));
            var combinedPermutations = validPermutations.Select(i => CombinePermutation(i.Item1, i.Item2));
            var res = new MineRegionPermutation(combinedPermutations, verboseLogging);
            if (verboseLogging)
            {
                Console.WriteLine($"Got a total of {res.Permutations.Count()} valid permutations.");
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