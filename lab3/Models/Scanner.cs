using System.Collections.Generic;

namespace lab3.Models
{
    public class Scanner
    {
        public List<Token> Scan(string text)
        {
            var tokens = new List<Token>();
            int i = 0;

            while (i < text.Length)
            {
                char c = text[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (c == ':' && i + 1 < text.Length && text[i + 1] == '=')
                {
                    tokens.Add(new Token(":=", TokenType.AssignOp));
                    i += 2;
                    continue;
                }

                switch (c)
                {
                    case ';':
                        tokens.Add(new Token(";", TokenType.Semicolon));
                        i++;
                        continue;
                    case '+':
                        tokens.Add(new Token("+", TokenType.Plus));
                        i++;
                        continue;
                    case '-':
                        tokens.Add(new Token("-", TokenType.Minus));
                        i++;
                        continue;
                    case '*':
                        tokens.Add(new Token("*", TokenType.Mul));
                        i++;
                        continue;
                    case '/':
                        tokens.Add(new Token("/", TokenType.Div));
                        i++;
                        continue;
                    case '(':
                        tokens.Add(new Token("(", TokenType.LParen));
                        i++;
                        continue;
                    case ')':
                        tokens.Add(new Token(")", TokenType.RParen));
                        i++;
                        continue;
                }

                if (c == '\'')
                {
                    int start = i;
                    i++;
                    while (i < text.Length && text[i] != '\'')
                        i++;
                    if (i < text.Length) i++;

                    string lexeme = text.Substring(start, i - start);
                    tokens.Add(new Token(lexeme, TokenType.CharConstant));
                    continue;
                }

                if (char.IsLetter(c))
                {
                    int start = i;
                    i++;
                    while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_'))
                    {
                        i++;
                    }
                    string lexeme = text.Substring(start, i - start);
                    tokens.Add(new Token(lexeme, TokenType.Identifier));
                    continue;
                }

                tokens.Add(new Token(c.ToString(), TokenType.Unknown));
                i++;
            }

            tokens.Add(new Token("", TokenType.EndOfInput));
            return tokens;
        }
    }
}
