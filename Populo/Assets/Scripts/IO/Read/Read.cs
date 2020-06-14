using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class Read : MonoBehaviour
{
    public static char Assignment = '=';
    public static char OpenObject = '{';
    public static char CloseObject = '}';
    public static char OpenList = '[';
    public static char CloseList = ']';
    public static char OpenPrimitive = '(';
    public static char ClosePrimitive = ')';
    public static char OpenString = '"';
    public static char CloseString = '"';
    public static char Escape = '\\';
    public static char Comma = ',';

    public static readonly HashSet<char> Dividers = new HashSet<char> { Assignment, OpenObject, CloseObject, OpenList, CloseList, OpenPrimitive, ClosePrimitive, OpenString, CloseString, Escape, Comma };

    public static string ReadFromResources(string directory, string file)
    {
        TextAsset text = (TextAsset)Resources.Load(Path.Combine(directory, file), typeof(TextAsset));
        return text.text;
    }

    public static string ReadFromFile(string directory, string file)
    {
        string path = Path.Combine(GameFolder.MainDirectory, directory, file) + IO.TypedObjectNotationExtension;
        if (!File.Exists(path)) throw new NoSuchFileException(directory, file);
        return File.ReadAllText(path);
    }

    public static MapModel ReadMapFromFile(string directory, string file)
    {
        string fileText = ReadFromFile(directory, file);
        FileIterator iterator = new FileIterator(Path.Combine(directory, file) + IO.TypedObjectNotationExtension, fileText);
        ReadMap.ObjectMap objectMap = ReadIntoMap.ReadIntoObjectMap("MapModel", iterator);
        return new MapModel(objectMap);
    }

    public static void ReadResource(string directory, string file, Type type)
    {
        string fileText = ReadFromResources(directory, file);
        FileIterator iterator = new FileIterator(Path.Combine(directory, file), fileText);
        ReadMap.ObjectMap objectMap = ReadIntoMap.ReadIntoObjectMap("MapModel", iterator);
        Activator.CreateInstance(type, new object[] { objectMap });
    }

    public class ReadException : UnityException
    {
        public ReadException(string message) : base(message) { }
        public ReadException(string filePath, int line, string message) : base($"{message} (at {filePath}, Line {line}))") { }
    }

    public class NoSuchFileException : ReadException
    {
        public NoSuchFileException(string directory, string file) : base($"No file named {file} in directory {directory}") { }
    }

    public class InvalidPrimitiveValueException: ReadException
    {
        public InvalidPrimitiveValueException(string filePath, int line, string message) : base(filePath, line, message) { }
    }

    public class NoSuchTypeException: ReadException
    {
        public NoSuchTypeException(string filePath, int line, string type) : base(filePath, line, $"No such type as {type}") { }
    }

    public class UnexpectedCharacterException : UnityException
    {
        public UnexpectedCharacterException(char c, string location, string context) : base($"Unexpected character {c} in {location}: {context}") { }
    }
}
