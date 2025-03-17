using System;
using System.Collections.Generic;


namespace lab3.Models
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _pos;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _pos = 0;
        }

        private Token CurrentToken => _tokens[_pos];
        private void NextToken() => _pos++;

        private void Match(TokenType type)
        {
            if (CurrentToken.TokenType == type)
            {
                NextToken();
            }
            else
            {
                throw new Exception($"Ожидалась лексема типа {type}, получено {CurrentToken.TokenType}");
            }
        }

        private bool IsCurrentToken(TokenType type)
        {
            return CurrentToken.TokenType == type;
        }

        public Node ParseS()
        {
            var sNode = new Node("S");

            if (!IsCurrentToken(TokenType.Identifier) && !IsCurrentToken(TokenType.CharConstant))
                throw new Exception("Ожидался идентификатор или символьная константа в начале выражения (S)");

            var aNode = new Node($"a: {CurrentToken.Lexeme}");
            sNode.AddChild(aNode);
            NextToken();

            if (!IsCurrentToken(TokenType.AssignOp))
                throw new Exception("Ожидался оператор ':='");

            var assignNode = new Node($":=");
            sNode.AddChild(assignNode);
            NextToken();

            var fNode = ParseF();
            sNode.AddChild(fNode);

            Match(TokenType.Semicolon);

            return sNode;
        }

        private Node ParseF()
        {
            var fNode = new Node("F");

            var tNode = ParseT();
            fNode.AddChild(tNode);

            var fPrime = ParseFPrime();
            fNode.AddChild(fPrime);

            return fNode;
        }

        private Node ParseFPrime()
        {
            var fPrime = new Node("F'");

            if (IsCurrentToken(TokenType.Plus))
            {
                var plusNode = new Node("+");
                fPrime.AddChild(plusNode);
                NextToken();

                var tNode = ParseT();
                fPrime.AddChild(tNode);

                var fPrime2 = ParseFPrime();
                fPrime.AddChild(fPrime2);
            }
            else
            {
                fPrime.AddChild(new Node("ε"));
            }
            return fPrime;
        }

        private Node ParseT()
        {
            var tNode = new Node("T");

            var eNode = ParseE();
            tNode.AddChild(eNode);

            var tPrime = ParseTPrime();
            tNode.AddChild(tPrime);

            return tNode;
        }

        private Node ParseTPrime()
        {
            var tPrime = new Node("T'");

            if (IsCurrentToken(TokenType.Mul) || IsCurrentToken(TokenType.Div))
            {
                var op = CurrentToken.TokenType == TokenType.Mul ? "*" : "/";
                var opNode = new Node(op);
                tPrime.AddChild(opNode);
                NextToken();

                var eNode = ParseE();
                tPrime.AddChild(eNode);

                var tPrime2 = ParseTPrime();
                tPrime.AddChild(tPrime2);
            }
            else
            {
                tPrime.AddChild(new Node("ε"));
            }
            return tPrime;
        }

        private Node ParseE()
        {
            var eNode = new Node("E");

            if (IsCurrentToken(TokenType.LParen))
            {
                var lparen = new Node("(");
                eNode.AddChild(lparen);
                NextToken();

                var fNode = ParseF();
                eNode.AddChild(fNode);

                if (!IsCurrentToken(TokenType.RParen))
                    throw new Exception("Ожидалась закрывающая скобка ')'");

                var rparen = new Node(")");
                eNode.AddChild(rparen);
                NextToken();
            }
            else if (IsCurrentToken(TokenType.Minus))
            {
                var minusNode = new Node("-");
                eNode.AddChild(minusNode);
                NextToken();

                if (!IsCurrentToken(TokenType.LParen))
                    throw new Exception("После '-' ожидалась '('");

                var lparen = new Node("(");
                eNode.AddChild(lparen);
                NextToken();

                var fNode = ParseF();
                eNode.AddChild(fNode);

                if (!IsCurrentToken(TokenType.RParen))
                    throw new Exception("Ожидалась ')' после (F");

                var rparen = new Node(")");
                eNode.AddChild(rparen);
                NextToken();
            }
            else
            {
                if (!IsCurrentToken(TokenType.Identifier) && !IsCurrentToken(TokenType.CharConstant))
                    throw new Exception("Ожидался идентификатор/символ (E)");

                var aNode = new Node($"a: {CurrentToken.Lexeme}");
                eNode.AddChild(aNode);
                NextToken();
            }

            return eNode;
        }
    }
}
