using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Types
{
    public static readonly HashSet<string> GenericTypes = new HashSet<string>
    {
        "Model"
    };

    private static readonly Dictionary<Type, string> TypeToName = new Dictionary<Type, string>()
    {
        { typeof(int), "Int" },
        { typeof(float), "Float" },
        { typeof(string), "String" },
        { typeof(Color32), "Color" },
        { typeof(Vector2Int), "Coordinates" }
    };

    public static string GetTypeName(this object obj)
    {
        return obj.GetType().ToStringCustom();
    }

    public static string ToStringCustom(this Type type)
    {
        if (TypeToName.ContainsKey(type)) return TypeToName[type];
        return type.ToString();
    }

    public static bool IsModel(this Type type)
    {
        return type.IsSubclassOf(typeof(Model));
    }

    public static bool IsArray(this Type type)
    {
        return type.IsSubclassOf(typeof(Array));
    }

    public static bool IsArrayOf(this Type type, Type elementType)
    {
        return type.GetElementType().IsSubclassOf(elementType);
    }

    public class TypeException : UnityException
    {
        public TypeException(string message) : base(message) { }
    }
}
