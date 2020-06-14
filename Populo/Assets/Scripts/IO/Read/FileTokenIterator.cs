using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileTokenIterator
{
    private readonly string filePath;
    private readonly List<List<string>> tokens = new List<List<string>>();

    private int baseLine = 0;
    private int currentLine = 0;
    private int currentTokenInLine = -1;

    public FileTokenIterator(string filePath)
    {
        this.filePath = filePath;
        System.IO.StreamReader file = new System.IO.StreamReader(filePath);
        string line;
        while ((line = file.ReadLine()) != null)
        {
            tokens.Add(ReadUtils.TokenizeString(line, Read.Dividers, tokens.Count));
        }

        file.Close();
    }

    public FileTokenIterator(string filePath, List<string> inputString)
    {
        this.filePath = filePath;
        foreach (string line in inputString)
        {
            tokens.Add(ReadUtils.TokenizeString(line, Read.Dividers, tokens.Count));
        }
    }

    private FileTokenIterator(string filePath, List<List<string>> tokens, int baseLine)
    {
        this.filePath = filePath;
        this.tokens = tokens;
        this.baseLine = baseLine;
    }

    public string Current()
    {
        if (currentTokenInLine < 0) return null;
        if (currentLine >= tokens.Count) return null;
        return tokens[currentLine][currentTokenInLine];
    }

    public bool HasNext()
    {
        return FindNextTokenLocation().line < tokens.Count;
    }

    public string PeekNext()
    {
        if (!HasNext()) return null;

        (int line, int tokenInLine) = FindNextTokenLocation();
        return tokens[line][tokenInLine];
    }

    public string Next()
    {
        MoveToNextToken();
        return Current();
    }

    public int GetCurrentLine()
    {
        return baseLine + currentLine;
    }

    public static FileTokenIterator MakeFileTokenIteratorForCurrentObject(FileTokenIterator tokenIterator, string objectName, string objectType)
    {
        int baseLineNumber = tokenIterator.GetCurrentLine();
        int currentLineNumber = tokenIterator.GetCurrentLine();
        List<List<string>> tokens = new List<List<string>>
        {
            new List<string>()
        };
        int objectRecursiveDepth = 1;

        while (tokenIterator.HasNext())
        {
            if (tokenIterator.Next() == "}") objectRecursiveDepth--;
            if (objectRecursiveDepth < 1) return new FileTokenIterator(tokenIterator.filePath, tokens, baseLineNumber);
            IncreaseCurrentLineToThisIterator();
            tokens[currentLineNumber - baseLineNumber].Add(tokenIterator.Current());
        }
        throw new MalformedItemException($"{objectType} {objectName}", "has no closing '}'", tokenIterator);

        void IncreaseCurrentLineToThisIterator()
        {
            while (currentLineNumber < tokenIterator.GetCurrentLine())
            {
                tokens.Add(new List<string>());
                currentLineNumber++;
            }
        }
    }

    public static FileTokenIterator MakeFileTokenIteratorForCurrentList(FileTokenIterator tokenIterator, string listName, string listType)
    {
        int baseLineNumber = tokenIterator.GetCurrentLine();
        int currentLineNumber = tokenIterator.baseLine;
        List<List<string>> tokens = new List<List<string>>
        {
            new List<string>()
        };

        while (tokenIterator.HasNext())
        {
            if (tokenIterator.Next() == "]") return new FileTokenIterator(tokenIterator.filePath, tokens, baseLineNumber);
            IncreaseCurrentLineToThisIterator();
            tokens[currentLineNumber - baseLineNumber].Add(tokenIterator.Current());
        }
        throw new MalformedItemException($"{listName} {listType}", "has no closing ']'", tokenIterator);

        void IncreaseCurrentLineToThisIterator()
        {
            while (currentLineNumber < tokenIterator.GetCurrentLine())
            {
                tokens.Add(new List<string>());
                currentLineNumber++;
            }
        }
    }

    public static FileTokenIterator MakeFileTokenIteratorForCurrentPrimitive(FileTokenIterator tokenIterator, string primitiveName, string primitiveType)
    {
        int baseLineNumber = tokenIterator.GetCurrentLine();
        int currentLineNumber = tokenIterator.baseLine;
        List<List<string>> tokens = new List<List<string>>
        {
            new List<string>()
        };

        while (tokenIterator.HasNext())
        {
            if (tokenIterator.Next() == ")") return new FileTokenIterator(tokenIterator.filePath, tokens, baseLineNumber);
            IncreaseCurrentLineToThisIterator();
            tokens[currentLineNumber - baseLineNumber].Add(tokenIterator.Current());
        }
        throw new MalformedItemException($"{primitiveName} {primitiveType}", "has no closing ')'", tokenIterator);

        void IncreaseCurrentLineToThisIterator()
        {
            while (currentLineNumber < tokenIterator.GetCurrentLine())
            {
                tokens.Add(new List<string>());
                currentLineNumber++;
            }
        }
    }

    public class FileTokenIteratorException : UnityException
    {
        public FileTokenIteratorException(FileTokenIterator tokenIterator, string message) : base($"{tokenIterator.filePath} (Line {tokenIterator.GetCurrentLine()}) : {message}") { }
        public FileTokenIteratorException(string filePath, int line, string message) : base($"{filePath} (Line {line}) : {message}") { }
    }

    public class FileCastException : FileTokenIteratorException
    {
        public FileCastException(string item, string type, FileTokenIterator tokenIterator) : base(tokenIterator, $"Illegal cast: {item} cannot be cast to {type}") { }
    }

    public class MalformedItemException : FileTokenIteratorException
    {
        public MalformedItemException(string item, string context, FileTokenIterator tokenIterator) : base(tokenIterator, $"Malformed {item}: {context}") { }
        public MalformedItemException(string item, string context, int line) : base("", line, $"Malformed {item}: {context}") { }
    }

    public class NoSuchTypeException : FileTokenIteratorException
    {
        public NoSuchTypeException(string type, FileTokenIterator tokenIterator) : base(tokenIterator, $"No such type as {type}") { }
    }

    public class UnexpectedCharacterException : FileTokenIteratorException
    {
        public UnexpectedCharacterException(char c, string location, string context, FileTokenIterator tokenIterator) : base(tokenIterator, $"Unexpected character {c} in {location}: {context}") { }
        public UnexpectedCharacterException(char c, string location, string context, string filePath, int line) : base(filePath, line, $"Unexpected character {c} in {location}: {context}") { }
    }

    private void MoveToNextToken()
    {
        (int line, int tokenInLine) = FindNextTokenLocation();
        currentLine = line;
        currentTokenInLine = tokenInLine;
    }

    private (int line, int tokenInLine) FindNextTokenLocation()
    {
        if (currentLine >= tokens.Count) return (currentLine, 0);

        int nextTokenInLine = currentTokenInLine + 1;
        if (nextTokenInLine < tokens[currentLine].Count) return (currentLine, nextTokenInLine);

        int nextLine = currentLine + 1;
        while (nextLine < tokens.Count)
        {
            if (tokens[nextLine].Count > 0) return (nextLine, 0);
            nextLine++;
        }
        return (nextLine, 0);
    }
}
