using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class CppScanner
{
    private static readonly HashSet<string> keywords = new HashSet<string>
    {
        "int", "float", "double", "char", "return", "if", "else", "while", "for", "void", "main","cout",
        "cin", "bool", "true", "false", "do", "switch", "case", "default","break","continue",
        "class", "public", "private", "protected", "namespace", "using", "std", "new", "delete",
        "include", "nullptr", "this", "virtual", "override", "const", "static", "friend", "typename"
    };

    public List<Token> Tokenize(string code)
    {
        List<Token> tokens = new List<Token>();
        Stack<char> symbolStack = new Stack<char>(); // For tracking unclosed symbols
        string[] lines = code.Split(new[] { '\n' }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            int index = 0;
            bool firstToken = true; // Track if it's the first token in the line

            while (index < line.Length)
            {
                char currentChar = line[index];

                // Match whitespace
                if (char.IsWhiteSpace(currentChar))
                {
                    if (currentChar == '\n')
                    {
                        tokens.Add(new Token("\\n", TokenType.Newline));
                    }
                    else
                    {
                        tokens.Add(new Token(currentChar.ToString(), TokenType.Whitespace));
                    }
                    index++;
                    continue;
                }

                // If it's the first non-whitespace token in the line, it should be a keyword or directive
                if (firstToken)
                {
                    firstToken = false; // We only do this check for the first token in each line

                    // Check if it’s a keyword
                    var keywordMatch = Regex.Match(line.Substring(index), @"^[a-zA-Z_][a-zA-Z0-9_]*");
                    if (keywordMatch.Success)
                    {
                        string keyword = keywordMatch.Value;
                        if (keywords.Contains(keyword))
                        {
                            tokens.Add(new Token(keyword, TokenType.Keyword));
                            index += keyword.Length;
                            continue;
                        }
                        else
                        {
                            tokens.Add(new Token(keyword, TokenType.Error, "Expected a keyword or directive at the beginning of the line"));
                            index += keyword.Length;
                            continue;
                        }
                    }
                    else if (currentChar == '#')
                    {
                        // Handle directives like #include
                        var directiveMatch = Regex.Match(line.Substring(index), @"^#\w+");
                        if (directiveMatch.Success)
                        {
                            tokens.Add(new Token(directiveMatch.Value, TokenType.Keyword));
                            index += directiveMatch.Length;
                            continue;
                        }
                        else
                        {
                            tokens.Add(new Token("#", TokenType.Error, "Unknown directive"));
                            index++;
                            continue;
                        }
                    }
                }

                // Match comments
                if (line.Substring(index).StartsWith("//"))
                {
                    var commentMatch = Regex.Match(line.Substring(index), @"^//.*");
                    tokens.Add(new Token(commentMatch.Value, TokenType.Comment));
                    index += commentMatch.Length;
                    continue;
                }
                else if (line.Substring(index).StartsWith("/*"))
                {
                    var commentMatch = Regex.Match(line.Substring(index), @"^/\*[\s\S]*?\*/");
                    if (commentMatch.Success)
                    {
                        tokens.Add(new Token(commentMatch.Value, TokenType.Comment));
                        index += commentMatch.Length;
                    }
                    else
                    {
                        tokens.Add(new Token("/*", TokenType.Error, "Unclosed comment"));
                        index += 2;
                    }
                    continue;
                }

                // Match special characters and track braces for unclosed braces error
                if ("{}()[],;".Contains(currentChar))
                {
                    tokens.Add(new Token(currentChar.ToString(), TokenType.SpecialCharacter));

                    // Track open and close symbols
                    if (currentChar == '{' || currentChar == '(')
                        symbolStack.Push(currentChar);
                    else if (currentChar == '}' || currentChar == ')')
                    {
                        if (symbolStack.Count == 0 ||
                            (currentChar == '}' && symbolStack.Peek() != '{') ||
                            (currentChar == ')' && symbolStack.Peek() != '('))
                        {
                            tokens.Add(new Token(currentChar.ToString(), TokenType.Error, "Unmatched closing brace or parenthesis"));
                        }
                        else
                        {
                            symbolStack.Pop();
                        }
                    }
                    index++;
                    continue;
                }

                // Match operators
                string operatorPattern = @"(\+\+|--|\+=|-=|\*=|/=|%=|&&|\|\||==|!=|<=|>=|->|::|<|>|=|\+|-|\*|/|&|\||\^|~|!)";
                var operatorMatch = Regex.Match(line.Substring(index), $"^{operatorPattern}");
                if (operatorMatch.Success)
                {
                    tokens.Add(new Token(operatorMatch.Value, TokenType.Operator));
                    index += operatorMatch.Length;
                    continue;
                }

                // Check for invalid identifiers (starting with a number followed by letters)
                var invalidIdentifierMatch = Regex.Match(line.Substring(index), @"^\d+[a-zA-Z_][a-zA-Z0-9_]*");
                if (invalidIdentifierMatch.Success)
                {
                    tokens.Add(new Token(invalidIdentifierMatch.Value, TokenType.Error, "Invalid identifier starting with a number"));
                    index += invalidIdentifierMatch.Length;
                    continue;
                }

                // Match numeric constants (integers and floats)
                var numberMatch = Regex.Match(line.Substring(index), @"^\d+(\.\d+)?");
                if (numberMatch.Success)
                {
                    tokens.Add(new Token(numberMatch.Value, TokenType.NumericConstant));
                    index += numberMatch.Length;
                    continue;
                }

                // Match identifiers and keywords
                var identifierMatch = Regex.Match(line.Substring(index), @"^[a-zA-Z_][a-zA-Z0-9_]*");
                if (identifierMatch.Success)
                {
                    string identifier = identifierMatch.Value;
                    TokenType type = keywords.Contains(identifier) ? TokenType.Keyword : TokenType.Identifier;
                    tokens.Add(new Token(identifier, type));
                    index += identifierMatch.Length;
                    continue;
                }

                // Match string literals with error handling
                if (currentChar == '"')
                {
                    var stringMatch = Regex.Match(line.Substring(index), "^\"(\\\\\"|[^\"])*\"");
                    if (stringMatch.Success)
                    {
                        tokens.Add(new Token(stringMatch.Value, TokenType.CharacterConstant));
                        index += stringMatch.Length;
                    }
                    else
                    {
                        tokens.Add(new Token("\"", TokenType.Error, "Unclosed string literal"));
                        index++;
                    }
                    continue;
                }

                // Match character literals with error handling
                if (currentChar == '\'')
                {
                    var charMatch = Regex.Match(line.Substring(index), @"^'(\\.|[^'])'");
                    if (charMatch.Success)
                    {
                        tokens.Add(new Token(charMatch.Value, TokenType.CharacterConstant));
                        index += charMatch.Length;
                    }
                    else
                    {
                        tokens.Add(new Token("'", TokenType.Error, "Unclosed character literal"));
                        index++;
                    }
                    continue;
                }

                // Unknown character
                tokens.Add(new Token(currentChar.ToString(), TokenType.Error, "Unknown character"));
                index++;
            }
        }

        // Check for any unmatched opening symbols left in the stack
        while (symbolStack.Count > 0)
        {
            char unclosedSymbol = symbolStack.Pop();
            string errorMsg = unclosedSymbol == '{' ? "Unclosed curly brace" : "Unclosed parenthesis";
            tokens.Add(new Token(unclosedSymbol.ToString(), TokenType.Error, errorMsg));
        }

        return tokens;
    }
}


