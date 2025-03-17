using System.Collections.Generic;

namespace lab3.Models
{
    public enum TokenType
    {
        Identifier,
        CharConstant,
        Plus, Minus, Mul, Div,
        AssignOp,
        Semicolon,
        LParen, RParen,
        Unknown,
        EndOfInput
    }

    public class Token
    {
        public int Index { get; set; }
        public string Lexeme { get; set; }
        public TokenType TokenType { get; set; }

        public Token(string lexeme, TokenType type)
        {
            Lexeme = lexeme;
            TokenType = type;
        }
    }
}
