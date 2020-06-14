using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string LowercaseFirstLetter(this string str)
    {
        return str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1);
    }

    public static string ExtractBetweenCharacters(this string str, char start, char end)
    {
        int startIndex = str.IndexOf(start);
        int endIndex = str.LastIndexOf(end);

        if (startIndex < 0 || endIndex < 0 || endIndex - startIndex - 1 < 1)
        {
            return string.Empty;
        }
        return str.Substring(startIndex + 1, endIndex - startIndex - 1);
    }
}
