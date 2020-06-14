using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReadPrimitives
{

    public static object ReadPrimitive(ReadMap.PrimitiveMap primitiveMap)
    {
        switch (primitiveMap.Type)
        {
            case "Color":
                return ReadColor(primitiveMap);
            case "Coordinates":
                return ReadCoordinates(primitiveMap);
            case "Float":
                return ReadFloat(primitiveMap);
            case "Int":
                return ReadInt(primitiveMap);
            case "String":
                return ReadString(primitiveMap);
            default:
                throw new Read.NoSuchTypeException(primitiveMap.FilePath, primitiveMap.Line, primitiveMap.Type);
        }
    }

    private static Color32 ReadColor(ReadMap.PrimitiveMap primitiveMap)
    {
        string[] values = primitiveMap.Value.Split(',');
        if (values.Length != 4) throw new ReadPrimitiveException(primitiveMap, $"{Types.MapTypeName(typeof(Color32).ToString())} must have 4 values from 0-255");
        
        int[] rgba = new int[4];
        for (int i = 0; i < rgba.Length; i++)
        {
            if (!int.TryParse(values[i].Trim(), out rgba[i])) throw new ReadPrimitiveException(primitiveMap, $"{Types.MapTypeName(typeof(Color32).ToString())} input {rgba[i]} is not an integer");
            CheckRGBAValueBelow256(rgba[i]);
        }

        return new Color32((byte)rgba[0], (byte)rgba[1], (byte)rgba[2], (byte)rgba[3]);

        void CheckRGBAValueBelow256(int rgbaValue)
        {
            if (rgbaValue > 255 || rgbaValue < 0) throw new ReadPrimitiveException(primitiveMap, $"rgba values in {Types.MapTypeName(typeof(Color32).ToString())} must be 0 to 255");
        }
    }

    private static Vector2Int ReadCoordinates(ReadMap.PrimitiveMap primitiveMap)
    {
        string [] values = primitiveMap.Value.Split(',');
        if (values.Length != 2) throw new Read.InvalidPrimitiveValueException(primitiveMap.FilePath, primitiveMap.Line, $"{Types.MapTypeName(typeof(Vector2Int).ToString())} must have 2 integer values");

        int[] xy = new int[2];
        for (int i = 0; i < xy.Length; i++)
        {
            if (!int.TryParse(values[i].Trim(), out xy[i])) throw new ReadPrimitiveException(primitiveMap, $"{Types.MapTypeName(typeof(Vector2Int).ToString())} input {xy[i]} is not an integer");
        }

        return new Vector2Int(xy[0], xy[1]);
    }

    private static float ReadFloat(ReadMap.PrimitiveMap primitiveMap)
    {
        if (float.TryParse(primitiveMap.Value, out float floatValue))
        {
            return floatValue;
        }
        throw new ReadPrimitiveException(primitiveMap, $"{primitiveMap.Value} is not a valid {Types.MapTypeName(typeof(float).ToString())}");
    }

    private static int ReadInt(ReadMap.PrimitiveMap primitiveMap)
    {
        if (int.TryParse(primitiveMap.Value, out int integer))
        {
            return integer;
        }
        throw new ReadPrimitiveException(primitiveMap, $"{primitiveMap.Value} is not a valid {Types.MapTypeName(typeof(int).ToString())}");
    }

    private static string ReadString(ReadMap.PrimitiveMap primitiveMap)
    {
        List<string> strings = new List<string>();
        string[] subStrings = primitiveMap.Value.Split(',');
        foreach (string subString in subStrings)
        {
            if (subString[0] != '"') throw new Read.InvalidPrimitiveValueException(primitiveMap.FilePath, primitiveMap.Line, $"Strings must start with a \" character");
            if (subString[subString.Length -1] != '"') throw new Read.InvalidPrimitiveValueException(primitiveMap.FilePath, primitiveMap.Line, $"Strings must end with a \" character");
            strings.Add(subString.ExtractBetweenCharacters('"', '"'));
        }
        return string.Join(string.Empty, strings);
    }

    public static Color32 ReadColor(FileTokenIterator tokenIterator)
    {
        int[] colorValues = new int[4];
        colorValues[0] = ReadInt(tokenIterator);
        for (int i = 1; i < colorValues.Length; i++)
        {
            if (tokenIterator.Next() != ",") throw new FileTokenIterator.MalformedItemException(new Color32().GetTypeName(), $"primitive values must be separated by commas", tokenIterator);
            colorValues[i] = ReadInt(tokenIterator);
        }
        CheckRGBAValuesBelow256();
        return new Color32((byte)colorValues[0], (byte)colorValues[1], (byte)colorValues[2], (byte)colorValues[3]);

        void CheckRGBAValuesBelow256()
        {
            foreach (int i in colorValues)
            {
                if (i > 255) throw new FileTokenIterator.MalformedItemException(new Color32().GetTypeName(), $"rgba values must be 0 to 255", tokenIterator);
            }
        }
    }

    public static Vector2Int ReadCoordinates(FileTokenIterator tokenIterator)
    {
        int x = ReadInt(tokenIterator);
        if (tokenIterator.Next() != ",") throw new FileTokenIterator.MalformedItemException(new Vector2Int().GetTypeName(), "primitive values must be separated by commas", tokenIterator);
        int y = ReadInt(tokenIterator);
        return new Vector2Int(x, y);
    }

    public static float ReadFloat(FileTokenIterator tokenIterator)
    {
        if (!tokenIterator.HasNext()) throw new FileTokenIterator.MalformedItemException(new float().GetTypeName(), $"expected {new float().GetTypeName()}", tokenIterator);
        if (float.TryParse(tokenIterator.Next(), out float output)) return output;
        else throw new FileTokenIterator.FileCastException(tokenIterator.Current(), new float().GetTypeName(), tokenIterator);
    }

    public static int ReadInt(FileTokenIterator tokenIterator)
    {
        if (!tokenIterator.HasNext()) throw new FileTokenIterator.MalformedItemException(new int().GetTypeName(), $"expected {new int().GetTypeName()}", tokenIterator);
        if (int.TryParse(tokenIterator.Next(), out int output)) return output;
        else throw new FileTokenIterator.FileCastException(tokenIterator.Current(), new int().GetTypeName(), tokenIterator);
    }

    public static string ReadString(FileTokenIterator tokenIterator)
    {
        List<string> strings = new List<string>();
        while (tokenIterator.HasNext())
        {
            if (strings.Count > 0)
            {
                if (!tokenIterator.HasNext() || tokenIterator.Next() != ",") throw new FileTokenIterator.MalformedItemException("".GetTypeName(), "primitive values must be separated by commas", tokenIterator);
            }
            strings.Add(ReadSubString(tokenIterator));
        }
        return string.Join("", strings);
    }

    private static string ReadSubString(FileTokenIterator tokenIterator)
    {
        if (!tokenIterator.HasNext() || tokenIterator.Next() != "\"") throw new FileTokenIterator.MalformedItemException("".GetTypeName(), "must start with a '\"'", tokenIterator);
        if (!tokenIterator.HasNext()) throw new FileTokenIterator.MalformedItemException("".GetTypeName(), "must contain a value", tokenIterator);
        string value = tokenIterator.Next();
        if (value == "\"") return "";
        if (!tokenIterator.HasNext() || tokenIterator.Next() != "\"") throw new FileTokenIterator.MalformedItemException("".GetTypeName(), "must end with a '\"'", tokenIterator);

        return value;
    }

    public class ReadPrimitiveException : Read.InvalidPrimitiveValueException
    {
        public ReadPrimitiveException(ReadMap.PrimitiveMap primitiveMap, string message) : base(primitiveMap.FilePath, primitiveMap.Line, message) { }
    }

}
