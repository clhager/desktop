using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ObjectExtensions
{
    public static T Apply<T>(this T item, Action<T> function)
    {
        function(item);
        return item;
    }
}
