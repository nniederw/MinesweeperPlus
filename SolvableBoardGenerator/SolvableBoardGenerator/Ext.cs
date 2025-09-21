using System.Collections;
using System.Numerics;

public static class Ext
{
    public static void Assert(bool b)
    {
        if (!b) throw new Exception();
    }
    public static IEnumerable<T> Foreach<T>(this IEnumerable<T> list, Action<T> action)
    {
        foreach (var i in list) { action(i); }
        return list;
    }
    public static T[] Foreach<T>(this T[] arr, Action<T> action)
    {
        foreach (var i in arr) { action(i); }
        return arr;
    }
    public static bool TrueForOne<T>(this IEnumerable<T> list, Predicate<T> match)
    {
        foreach (var item in list)
        {
            if (match(item))
            {
                return true;
            }
        }
        return false;
    }
    public static T[,] Foreach<T>(this T[,] arr, Action<T> action)
    {
        foreach (var i in arr) { action(i); }
        return arr;
    }
    public static bool InBound<T>(this T[] arr, int i)
        => i >= 0 && i < arr.Length;
    public static bool InBound<T>(this T[,] arr, int i, int j)
        => i >= 0 && i < arr.GetLength(0) && j >= 0 && j < arr.GetLength(1);
    public static void Fill<T>(this T[] arr, T t)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            arr[i] = t;
        }
    }
    public static void Fill<T>(this T[,] arr, T t)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                arr[i, j] = t;
            }
        }
    }
    /*public static byte[] SingleDArray(this byte[,] arr) => arr.SingleDArray(sizeof(byte));
    public static sbyte[] SingleDArray(this sbyte[,] arr) => arr.SingleDArray(sizeof(sbyte));
    public static short[] SingleDArray(this short[,] arr) => arr.SingleDArray(sizeof(short));
    public static ushort[] SingleDArray(this ushort[,] arr) => arr.SingleDArray(sizeof(ushort));
    public static int[] SingleDArray(this int[,] arr) => arr.SingleDArray(sizeof(int));
    public static uint[] SingleDArray(this uint[,] arr) => arr.SingleDArray(sizeof(uint));
    public static long[] SingleDArray(this long[,] arr) => arr.SingleDArray(sizeof(long));
    public static ulong[] SingleDArray(this ulong[,] arr) => arr.SingleDArray(sizeof(ulong));
    public static char[] SingleDArray(this char[,] arr) => arr.SingleDArray(sizeof(char));
    public static float[] SingleDArray(this float[,] arr) => arr.SingleDArray(sizeof(float));
    public static double[] SingleDArray(this double[,] arr) => arr.SingleDArray(sizeof(double));
    public static decimal[] SingleDArray(this decimal[,] arr) => arr.SingleDArray(sizeof(decimal));
    public static bool[] SingleDArray(this bool[,] arr) => arr.SingleDArray(sizeof(bool));
    public static T[] SingleDArray<T>(this T[,] arr, int sizeofType)
    {
        var res = new T[arr.Length];
        Buffer.BlockCopy(arr, 0, res, 0, arr.Length * sizeofType);
        return res;
    }*/
    public static IEnumerable<T> AsEnumerable<T>(this T[,] arr)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                yield return arr[i, j];
            }
        }
    }
    public static B[,] ArrayCast<A, B>(this A[,] arr) //todo try to righly implement for from int to sbyte
    {
        var res = new B[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                res[i, j] = (B)(object)arr[i, j];
            }
        }
        return res;
    }
    public static B[,] Select<A, B>(this A[,] arr, Func<A, B> f) //todo try to righly implement for from int to sbyte
    {
        var res = new B[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                res[i, j] = f(arr[i, j]);
            }
        }
        return res;
    }
    public static IEnumerable<K> KeyIntersection<K, V>(this IReadOnlyDictionary<K, V> dict1, IReadOnlyDictionary<K, V> dict2)
    {
        if (dict1.Count > dict2.Count)
        {
            var tmp = dict1;
            dict1 = dict2;
            dict2 = tmp;
        }
        //dict1 < dict2
        foreach (var k in dict1.Keys)
        {
            if (dict2.ContainsKey(k))
            {
                yield return k;
            }
        }
    }
    public static IEnumerable<(A, B)> CartesianProduct<A, B>(this IEnumerable<A> aElements, IEnumerable<B> bElements) => aElements.SelectMany(a => bElements, (a, b) => (a, b));
    public static IEnumerator<T> ToGeneric<T>(this IEnumerator enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return (T)enumerator.Current;
        }
    }
    public static IEnumerable<T> AsSingleEnumerable<T>(this T element)
    {
        yield return element;
    }
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Remove(item);
        }
    }
    public static IEnumerable<T> ExceptT<T>(this IEnumerable<T> elements, T value)
    {
        foreach (var element in elements)
        {
            if (!element.Equals(value))
            {
                yield return element;
            }
        }
    }
    public static IEnumerable<T> ExceptT<T>(this IEnumerable<T> elements, T value1, T value2)
    {
        foreach (var element in elements)
        {
            if (!element.Equals(value1) && !element.Equals(value2))
            {
                yield return element;
            }
        }
    }
    public static void ConsoleWriteColor(string msg, ConsoleColor? backColor = null, ConsoleColor? forColor = null)
    {
        var oldbc = Console.BackgroundColor;
        var oldfc = Console.ForegroundColor;
        if (backColor != null)
        {
            Console.BackgroundColor = backColor.Value;
        }
        if (forColor != null)
        {
            Console.ForegroundColor = forColor.Value;
        }
        Console.Write(msg);
        Console.BackgroundColor = oldbc;
        Console.ForegroundColor = oldfc;
    }
}