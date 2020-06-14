using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadUtils
{
    public static List<string> TokenizeString(string inputString, HashSet<char> dividers, int line)
    {
        List<string> tokens = new List<string>();
        List<char> currentString = new List<char>();
        bool inString = false;

        IEnumerator<char> chars = inputString.GetEnumerator();
        while (chars.MoveNext())
        {
            char c = chars.Current;

            if (c == '\\') {
                HandleEscapeCharacter();
                continue;
            }
            if (c == '\"') {
                HandleStringQuoteCharacter();
                continue;
            }
            if (dividers.Contains(c) && !inString)
            {
                AddCurrentStringToTokens();
                AddCharacterToTokens(c);
                continue;
            }
            if (char.IsWhiteSpace(c) && !inString)
            {
                AddCurrentStringToTokens();
                continue;
            }
            if (char.IsControl(c)) {
                HandleControlCharacter();
                continue;
            }

            currentString.Add(c);
        }
        AddCurrentStringToTokens();

        return tokens;

        void HandleEscapeCharacter()
        {
            if (!chars.MoveNext()) throw new Read.UnexpectedCharacterException('\\', $"line {line}", "must have another character to escape");
            currentString.Add(chars.Current);
        }

        void HandleStringQuoteCharacter()
        {
            if (inString)
            {
                inString = false;
                AddCurrentStringToTokens();
                AddCharacterToTokens('\"');
            } 
            else
            {
                inString = true;
                AddCharacterToTokens('\"');
                AddCurrentStringToTokens();
            }
        }

        void HandleControlCharacter()
        {
            if (!inString) AddCurrentStringToTokens();
        }

        void AddCharacterToTokens(char character)
        {
            tokens.Add(character.ToString());
        }

        void AddCurrentStringToTokens()
        {
            if (currentString.Count > 0) tokens.Add(new string(currentString.ToArray()));
            currentString.Clear();
        }
    }
}
