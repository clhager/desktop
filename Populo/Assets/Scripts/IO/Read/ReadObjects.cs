using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadObjects : MonoBehaviour
{

    public static Dictionary<string, VariableTokenIterator> ReadObject(string name, FileTokenIterator tokenIterator)
    {
        Dictionary<string, VariableTokenIterator> map = new Dictionary<string, VariableTokenIterator>();

        while(tokenIterator.HasNext())
        {
            string variableName = tokenIterator.Next();
            CheckForAssignment(variableName);
            string variableType = GetVariableType(variableName);
            FileTokenIterator variableTokenIterator = GetTokenIteratorForVariableValue(variableName, variableType);
            map.Add(variableName, new VariableTokenIterator(variableType, variableTokenIterator));
            if (IsEndOfObject()) break;
        }
        return map;

        void CheckForAssignment(string variableName)
        {
            if (!tokenIterator.HasNext()) throw new FileTokenIterator.MalformedItemException($"variable {variableName}", "must be assigned", tokenIterator);
            if (tokenIterator.Next() != "=") throw new FileTokenIterator.MalformedItemException(
                $"variable {variableName}", 
                $"must be assigned a value with '=', not {tokenIterator.Current()}", 
                tokenIterator
            );
        }

        string GetVariableType(string variableName)
        {
            if (!tokenIterator.HasNext()) throw new FileTokenIterator.MalformedItemException($"variable {variableName}", "must have a type", tokenIterator);
            return tokenIterator.Next();
        }

        FileTokenIterator GetTokenIteratorForVariableValue(string variableName, string variableType)
        {
            switch (tokenIterator.Next())
            {
                case "{":
                    return FileTokenIterator.MakeFileTokenIteratorForCurrentObject(tokenIterator, variableName, variableType);
                case "[":
                    return FileTokenIterator.MakeFileTokenIteratorForCurrentList(tokenIterator, variableName, variableType);
                case "(":
                    return FileTokenIterator.MakeFileTokenIteratorForCurrentPrimitive(tokenIterator, variableName, variableType);
                default:
                    throw new FileTokenIterator.MalformedItemException($"object {name}", "must be an object, list, or primitive", tokenIterator);
            }
        }

        bool IsEndOfObject()
        {
            if (!tokenIterator.HasNext()) return true;
            if (tokenIterator.Next() != ",") throw new FileTokenIterator.MalformedItemException($"object {name}", "must separate variables with commas", tokenIterator);
            if (!tokenIterator.HasNext()) return true;
            return false;
        }
    }

    public static FileTokenIterator TryGet(Dictionary<string, VariableTokenIterator> map, string key, string objectName, string objectType, string variableType, int line)
    {
        if (map.TryGetValue(key, out VariableTokenIterator value))
        {
            if (variableType != value.type)
            {
                throw new FileTokenIterator.MalformedItemException($"{objectType} {objectName}", $"expected attribute {key} to be of type {variableType}", line);
            }
            return value.tokenIterator;
        }
        else
        {
            throw new FileTokenIterator.MalformedItemException($"{objectType} {objectName}", $"has no attribute {key}", line);
        }
    }
    public class VariableTokenIterator
    {
        public string type;
        public FileTokenIterator tokenIterator;

        public VariableTokenIterator(string type, FileTokenIterator tokenIterator)
        {
            this.type = type;
            this.tokenIterator = tokenIterator;
        }
    }
}
