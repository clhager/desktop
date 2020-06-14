using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static int AddThenGetIndex<T>(this List<T> list, T item)
    {
        list.Add(item);
        return list.Count - 1;
    }

    public static T[] Concatenate<T>(this T[] arr, T[] other)
    {
        List<T> concatenatedList = new List<T>();
        concatenatedList.AddRange(arr);
        concatenatedList.AddRange(other);
        return concatenatedList.ToArray();
    }

    public static Vector3 Vector3(this Vector2 vector, float height)
    {
        return new Vector3(vector.x, height, vector.y);
    }

    

    public static string GetTypeName(this int _)
    {
        return "Int";
    }

    public static string GetTypeName(this float _)
    {
        return "Float";
    }

    public static string GetTypeName(this string _)
    {
        return "String";
    }

    public static string GetTypeName(this Color32 _)
    {
        return "Color";
    }

    public static string GetTypeName(this Vector2Int _)
    {
        return "Vector2Int";
    }

    public static bool IsEven(this int integer)
    {
        return integer % 2 == 0;
    }

    public static bool ListEquals<T>(this List<T> list, List<T> other)
    {
        if (list.Count != other.Count) return false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(other[i]))
            {
                return false;
            }
        }
        return true;
    }

    
}
