public static class Combinatorics
{
    /// <summary>
    /// Iteratively generates all combinations of bools of k true values.
    /// </summary>
    public static IEnumerable<bool[]> GetCombinationsIterative(uint k, uint n)
    {
        if (k < 0 || k > n)
        {
            throw new ArgumentOutOfRangeException(nameof(k));
        }
        if (k == 0)
        {
            yield return new bool[n];
            yield break;
        }
        if (k == n)
        {
            var result = new bool[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = true;
            }
            yield return result;
            yield break;
        }
        // Index array: [0,1,2,...,k-1]
        var indices = new int[k];
        for (int i = 0; i < k; i++)
        {
            indices[i] = i;
        }
        while (true)
        {
            var combo = new bool[n];
            for (int i = 0; i < k; i++)
            {
                combo[indices[i]] = true;
            }
            yield return combo;
            int incrementPos = (int)k - 1;
            while (incrementPos >= 0 && indices[incrementPos] == n - k + incrementPos)
            {
                incrementPos--;
            }
            if (incrementPos < 0)
            {
                yield break;
            }
            indices[incrementPos]++;
            for (int j = incrementPos + 1; j < k; j++)
            {
                indices[j] = indices[j - 1] + 1;
            }
        }
    }
    /// <summary>
    /// Iteratively generates all combinations of size k from source[0..n-1].
    /// </summary>
    public static IEnumerable<List<T>> GetCombinationsIterative<T>(IList<T> source, uint k)
    {
        int n = source.Count;
        if (k < 0 || k > n)
            throw new ArgumentOutOfRangeException(nameof(k));

        if (k == 0)
        {
            yield return new List<T>();
            yield break;
        }
        if (k == n)
        {
            yield return new List<T>(source);
            yield break;
        }
        // Index array: [0,1,2,...,k-1]
        var indices = new int[k];
        for (int i = 0; i < k; i++)
        {
            indices[i] = i;
        }
        while (true)
        {
            var combo = new List<T>((int)k);
            for (int i = 0; i < k; i++)
            {
                combo.Add(source[indices[i]]);
            }
            yield return combo;
            int incrementPos = (int)k - 1;
            while (incrementPos >= 0 && indices[incrementPos] == n - k + incrementPos)
            {
                incrementPos--;
            }
            if (incrementPos < 0)
            {
                yield break;
            }
            indices[incrementPos]++;
            for (int j = incrementPos + 1; j < k; j++)
            {
                indices[j] = indices[j - 1] + 1;
            }
        }
    }
    /// <summary>
    /// Returns 
    /// </summary>
    public static IEnumerable<(uint a, uint b)> AllDouble32bNaturalNumbers()
    {
        uint a = 0;
        uint b = 0;
        yield return (a, b);
        while (a < uint.MaxValue)
        {
            if (a == 0)
            {
                a = b + 1;
                b = 0;
            }
            else
            {
                a--;
                b++;
            }
            yield return (a, b);
        }
    }
    public static IEnumerable<(uint a, uint b)> AllDouble32bNaturalNumbersGreaterThan(uint n)
        => AllDouble32bNaturalNumbers().Where((i) => i.a > n && i.b > n);
}