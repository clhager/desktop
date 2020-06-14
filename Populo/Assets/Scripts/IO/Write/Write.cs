using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Write
{
    private static readonly HashSet<char> IndentOnCharacter = new HashSet<char> { '{', '[' };
    private static readonly HashSet<char> DeindentOnCharacter = new HashSet<char> { '}', ']' };
    private static readonly HashSet<char> NewlineAfterCharacter = new HashSet<char> { '{', '[', ',' };
    private static readonly HashSet<char> NewlineBeforeCharacter = new HashSet<char> { '}', ']' };
    private static readonly Dictionary<char, char> NoNewlineWithinCharacters = new Dictionary<char, char>
    {
        { '(', ')' }
    };

    public static string PrettyPrint(string input)
    {
        List<string> prettyPrintString = new List<string>();
        string currentString = "";
        int indent = 0;
        List<char> noNewlines = new List<char>();

        foreach (char character in input)
        {
            if (NoNewlineWithinCharacters.ContainsKey(character)) noNewlines.Add(character);
            if (!noNewlines.IsEmpty() && NoNewlineWithinCharacters[noNewlines.Last()] == character) noNewlines.RemoveLast();
            if (IndentOnCharacter.Contains(character)) indent++;
            if (DeindentOnCharacter.Contains(character)) indent--;
            if (NewlineBeforeCharacter.Contains(character)) NewLine();
            currentString += character;
            if (NewlineAfterCharacter.Contains(character)) NewLine();
        }
        NewLine();
        return string.Join("\n", prettyPrintString);

        void NewLine()
        {
            if (noNewlines.Count < 1) {
                if (!string.IsNullOrWhiteSpace(currentString))
                {
                    prettyPrintString.Add(currentString);
                }
                currentString = new string('\t', Mathf.Max(indent, 0));
            }
        }
    }

    public static void WriteObjectToFile(string directory, string file, object contents)
    {
        WriteFile(directory, file, $"{{{contents.ToString()}}}\n");
    }

    public static void WriteFile(string directory, string file, string contents)
    {
        File.WriteAllText(Path.Combine(directory, file) + IO.TypedObjectNotationExtension, PrettyPrint(contents));

    }


}
