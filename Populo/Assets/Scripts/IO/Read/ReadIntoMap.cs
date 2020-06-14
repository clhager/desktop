using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadIntoMap
{
    public static ReadMap.ObjectMap ReadIntoObjectMap(string type, FileIterator fileIterator)
    {
        Dictionary<string, ReadMap.ObjectMap> objects = new Dictionary<string, ReadMap.ObjectMap>();
        Dictionary<string, ReadMap.ObjectList> objectLists = new Dictionary<string, ReadMap.ObjectList>();
        Dictionary<string, ReadMap.PrimitiveMap> primitives = new Dictionary<string, ReadMap.PrimitiveMap>();
        Dictionary<string, ReadMap.PrimitiveList> primitiveLists = new Dictionary<string, ReadMap.PrimitiveList>();

        if (!fileIterator.HasNext() || !AreEqual(fileIterator.Next(), Read.OpenObject)) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"object must be opened with '{Read.OpenObject}'");
        
        int line = fileIterator.Line;
        while (fileIterator.HasNext())
        {
            string variableName = fileIterator.Next();
            if (AreEqual(variableName, Read.CloseObject)) return FormatReturn();

            if (!fileIterator.HasNext() || !AreEqual(fileIterator.Next(), Read.Assignment)) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"variable name must be followed by assignment");
            
            if (!fileIterator.HasNext()) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"variable assignment must be followed by a valid variable type");
            string variableType = fileIterator.Next();

            if (!fileIterator.HasNext()) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"variable type must be followed by valid variable declaration");
            if (AreEqual(fileIterator.PeekNext(), Read.OpenObject))
            {
                objects.Add(variableName, ReadIntoObjectMap(variableType, fileIterator));
            }
            else if (AreEqual(fileIterator.PeekNext(), Read.OpenList))
            {
                (ReadMap.ObjectList objectList, ReadMap.PrimitiveList primitiveList) = ReadList(variableType, fileIterator.Line, fileIterator);
                if (objectList.ObjectItems.Count < 1)
                {
                    primitiveLists.Add(variableName, primitiveList);
                }
                else if (primitiveList.PrimitiveItems.Count < 1)
                {
                    objectLists.Add(variableName, objectList);
                }
                else
                {
                    throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"array {variableName} must not contain both objects and primitives");
                }
            }
            else if (AreEqual(fileIterator.PeekNext(), Read.OpenPrimitive))
            {
                primitives.Add(variableName, ReadIntoPrimitiveMap(variableType, fileIterator.Line, fileIterator));
            }
            else
            {
                throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"unrecognized variable declaration for {variableType} {variableName}");
            }

            if (!fileIterator.HasNext()) break;
            fileIterator.Next();
            if (AreEqual(fileIterator.Current(), Read.CloseObject)) return FormatReturn();
            else if (!AreEqual(fileIterator.Current(), Read.Comma)) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"list elements must be separated by '{Read.Comma}'");
        }
        throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"object must be closed with '{Read.CloseObject}'");

        ReadMap.ObjectMap FormatReturn()
        {
            return new ReadMap.ObjectMap(objects, objectLists, primitives, primitiveLists, type, fileIterator.FilePath, line);
        }
    }

    private static (ReadMap.ObjectList, ReadMap.PrimitiveList) ReadList(string type, int line, FileIterator fileIterator)
    {
        (List<ReadMap.ObjectMap> objectMaps, List<ReadMap.PrimitiveMap> primitiveMaps) = (new List<ReadMap.ObjectMap>(), new List<ReadMap.PrimitiveMap>());

        if (!fileIterator.HasNext() || !AreEqual(fileIterator.Next(), Read.OpenList)) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"object must be opened with '{Read.OpenList}'");
        while (fileIterator.HasNext())
        {
            if (AreEqual(fileIterator.PeekNext(), Read.CloseList))
            {
                fileIterator.Next();
                return FormatReturn();
            }
            else if (AreEqual(fileIterator.PeekNext(), Read.OpenObject))
            {
                objectMaps.Add(ReadIntoObjectMap(type, fileIterator));
            }
            else if (AreEqual(fileIterator.PeekNext(), Read.OpenPrimitive))
            {
                primitiveMaps.Add(ReadIntoPrimitiveMap(type, fileIterator.Line, fileIterator));
            }
            else
            {
                List<string> tokens = new List<string>();
                while (!AreEqual(fileIterator.PeekNext(), Read.CloseList) || !AreEqual(fileIterator.PeekNext(), Read.Comma))
                {
                    if (!fileIterator.HasNext()) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"lists must be closed with '{Read.CloseList}'");
                    tokens.Add(fileIterator.Next());
                }
                primitiveMaps.Add(new ReadMap.PrimitiveMap(string.Join("", tokens), type, fileIterator.FilePath, fileIterator.Line));
            }

            if (!fileIterator.HasNext()) break;
            fileIterator.Next();

            if (AreEqual(fileIterator.Current(), Read.CloseList)) return FormatReturn();
            else if (!AreEqual(fileIterator.Current(), Read.Comma)) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"list elements must be separated by '{Read.Comma}'");
        }
        throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"lists must be closed with '{Read.CloseList}'");

        (ReadMap.ObjectList, ReadMap.PrimitiveList) FormatReturn()
        {
            return (new ReadMap.ObjectList(objectMaps, type, fileIterator.FilePath, line), new ReadMap.PrimitiveList(primitiveMaps, type, fileIterator.FilePath, line));
        }
    }

    private static ReadMap.PrimitiveMap ReadIntoPrimitiveMap(string type, int line, FileIterator fileIterator)
    {
        List<string> tokens = new List<string>();

        if (!fileIterator.HasNext() || !AreEqual(fileIterator.Next(), Read.OpenPrimitive)) throw new Read.ReadException(fileIterator.FilePath, fileIterator.Line, $"object must be opened with '{Read.OpenPrimitive}'");
        while (fileIterator.HasNext())
        {
            if (!AreEqual(fileIterator.Next(), Read.ClosePrimitive)) tokens.Add(fileIterator.Current());
            else return new ReadMap.PrimitiveMap(string.Join("", tokens), type, fileIterator.FilePath, fileIterator.Line);
        }
        throw new Read.ReadException(fileIterator.FilePath, line, $"primitives must be closed with '{Read.ClosePrimitive}'");
    }

    private static bool AreEqual(string token, char character)
    {
        return token == character.ToString();
    }
}
