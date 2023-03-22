using System;
using System.Collections;
using System.Collections.Generic;

public static class Ext
{
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
    public static T[,] Foreach<T>(this T[,] arr, Action<T> action)
    {
        foreach (var i in arr) { action(i); }
        return arr;
    }
    public static bool InBound<T>(this T[] arr, int i)
        => i >= 0 && i < arr.Length;
    public static bool InBound<T>(this T[,] arr, int i, int j)
        => i >= 0 && i < arr.GetLength(0) && j >= 0 && j < arr.GetLength(1);
}