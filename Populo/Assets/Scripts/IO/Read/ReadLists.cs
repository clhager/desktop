using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadLists
{
    public static List<Color32> ReadColorList(string name, FileTokenIterator tokenIterator)
    {
        return new List<Color32>(
            from iterator
            in ReadListElements(name, new Color32().GetTypeName(), tokenIterator)
            select ReadPrimitives.ReadColor(iterator)
        );
    }

    public static List<float> ReadFloatList(string name, FileTokenIterator tokenIterator)
    {
        return new List<float>(
            from iterator
            in ReadListElements(name, new float().GetTypeName(), tokenIterator)
            select ReadPrimitives.ReadFloat(iterator)
        );
    }

    public static List<int> ReadIntList(string name, FileTokenIterator tokenIterator)
    {
        return new List<int>(
            from iterator
            in ReadListElements(name, new int().GetTypeName(), tokenIterator)
            select ReadPrimitives.ReadInt(iterator)
        );
    }

    public static List<string> ReadStringList(string name, FileTokenIterator tokenIterator)
    {
        return new List<string>(
            from iterator
            in ReadListElements(name, "".GetTypeName(), tokenIterator)
            select ReadPrimitives.ReadString(iterator)
        );
    }

    private static FileTokenIterator[] ReadListElements(string listName, string listType, FileTokenIterator tokenIterator)
    {
        List<FileTokenIterator> listElements = new List<FileTokenIterator>();
        while (tokenIterator.HasNext())
        {
            FileTokenIterator nextListElement = ReadListElement(listName, listType, tokenIterator);
            if (nextListElement == null) break;
            listElements.Add(nextListElement);
            if (IsEndOfList()) break;
        }
        return listElements.ToArray();

        bool IsEndOfList()
        {
            if (!tokenIterator.HasNext()) return true;
            if (tokenIterator.Next() == ",") return false;
            else throw new FileTokenIterator.MalformedItemException($"list {listName}", "must have commas to separate list elements", tokenIterator);
        }
    }

    private static FileTokenIterator ReadListElement(string listName, string listType, FileTokenIterator tokenIterator)
    {
        if (!tokenIterator.HasNext()) throw new FileTokenIterator.MalformedItemException($"list {listName}", "has no closing ']'", tokenIterator);

        switch (tokenIterator.Next())
        {
            case "=":
                throw new FileTokenIterator.UnexpectedCharacterException('=', $"list {listName}", "cannot use assignment in list", tokenIterator);
            case "[":
                throw new FileTokenIterator.UnexpectedCharacterException('[', $"list {listName}", "cannot declare list in list", tokenIterator);
            case "}":
                throw new FileTokenIterator.UnexpectedCharacterException('}', $"list {listName}", "cannot close undeclared object in list", tokenIterator);
            case ")":
                throw new FileTokenIterator.UnexpectedCharacterException(')', $"list {listName}", "cannot declare primitive in list", tokenIterator);
            case ",":
                throw new FileTokenIterator.UnexpectedCharacterException(',', $"list {listName}", "cannot have empty element in list", tokenIterator);

            //case "{":
                //return FileTokenIterator.MakeFileTokenIteratorForCurrentObject(tokenIterator, $"{listName}", listType); ;
            case "(":
                return FileTokenIterator.MakeFileTokenIteratorForCurrentPrimitive(tokenIterator, $"{listName}", listType);

            default:
                throw new FileTokenIterator.MalformedItemException($"list {listName}", $"unrecognized item {tokenIterator.Current()}, must declare list elements as objects or primitives", tokenIterator);
        }
    }
}
