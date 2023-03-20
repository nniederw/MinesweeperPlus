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
}