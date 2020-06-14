using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{

    public static bool IsEmpty<T>(this List<T> list)
    {
        return list.Count < 1;
    }

    public static T First<T>(this List<T> list)
    {
        return list[0];
    }

    public static T Last<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }

    public static void RemoveLast<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            list.RemoveAt(list.Count - 1);
        }
    }

    public static List<T> ListOfN<T>(this T item, int n)
    {
        List<T> list = new List<T>();
        for (int i = 0; i < n; i++) list.Add(item);
        return list;
    }

    public static List<T> Flatten<T>(this List<(T, T)> list)
    {
        List<T> flattenedList = new List<T>();
        foreach ((T, T) items in list)
        {
            flattenedList.Add(items.Item1);
            flattenedList.Add(items.Item2);
        }
        return flattenedList;
    }

    public static List<T> Flatten<T>(this List<(T, T, T)> list)
    {
        List<T> flattenedList = new List<T>();
        foreach ((T, T, T) items in list)
        {
            flattenedList.Add(items.Item1);
            flattenedList.Add(items.Item2);
            flattenedList.Add(items.Item3);
        }
        return flattenedList;
    }
}
