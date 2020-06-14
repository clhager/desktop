using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularOrderedSet<T>
{
    private readonly T[] array;

    public ModularOrderedSet(T[] array)
    {
        this.array = array;
    }

    public T this[int i]
    {
        get { return array[Mod(i, array.Length)]; }
        set { array[Mod(i, array.Length)] = value; }
    }

    public int IndexOf(T item)
    {
        return Array.IndexOf(array, item);
    }

    public T GetNext(T item)
    {
        return this[IndexOf(item) + 1];
    }

    public T GetPrevious(T item)
    {
        return this[IndexOf(item) - 1];
    }

    public IEnumerable<T> GetEnumerable()
    {
        return array;
    }

    public IEnumerator GetEnumerator()
    {
        return array.GetEnumerator();
    }

    private int Mod(int number, int modulo)
    {
        return (number + modulo) % modulo;
    }
}
