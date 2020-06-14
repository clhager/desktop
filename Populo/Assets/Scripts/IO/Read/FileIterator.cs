using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileIterator
{
    public string FilePath { get; set; }
    public int Line { get { return currentLine; } }
    private readonly List<List<string>> tokens = new List<List<string>>();

    private int currentLine = 0;
    private int currentTokenInLine = -1;

    public FileIterator(string filePath, string input)
    {
        FilePath = filePath;

        string[] lines = input.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            tokens.Add(ReadUtils.TokenizeString(lines[i], Read.Dividers, i));
        }
    }

    public string Current()
    {
        if (currentTokenInLine < 0) return null;
        if (currentLine >= tokens.Count) return null;
        return tokens[currentLine][currentTokenInLine];
    }

    public bool HasNext()
    {
        (int line, int tokenInLine) = FindNextTokenLocation();
        return line > currentLine || tokenInLine > currentTokenInLine;
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

    private void MoveToNextToken()
    {
        (currentLine, currentTokenInLine) = FindNextTokenLocation();
    }

    private (int line, int tokenInLine) FindNextTokenLocation()
    {
        int line = currentLine;
        int tokenInLine = currentTokenInLine;

        if (line < tokens.Count)
        {
            tokenInLine++;
            if (tokenInLine < tokens[currentLine].Count) return (line, tokenInLine);
            tokenInLine = 0;

            do
            {
                line++;
                if (line >= tokens.Count) break;
            } while (tokens[line].Count < 1);
        }
        return (line, tokenInLine);
    }
}
