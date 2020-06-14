using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link<T>
{
    private readonly Dictionary<T, T> link = new Dictionary<T, T>();

    public Link() { }

    public Link(Dictionary<T, T> dictionary)
    {
        foreach (KeyValuePair<T, T> pair in dictionary)
        {
            Add(pair.Key, pair.Value);
        }
    }

    public void Add(T key, T value)
    {
        link.Add(key, value);
        link.Add(value, key);
    }

    public T Get(T key)
    {
        return link[key];
    }
}
