namespace Therefore.Engine.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Therefore.Engine.Expressions;

    public sealed class Scanner
    {
        private readonly Dictionary<TokenType, Func<string, int, int?>> terminals = new Dictionary<TokenType, Func<string, int, int?>>
        {
            { TokenType.Variable, MakeRegex(@"\w+") },
            { TokenType.LeftParenthesis, MakeLiteral('(') },
            { TokenType.RightParenthesis, MakeLiteral(')') },
            { TokenType.UnaryOperator, MakeLiteral('~') },
            { TokenType.EOF, (source, location) =>
                {
                    return location == source.Length ? (int?)0 : null;
                }
            },
        };

        public Scanner(string[] binaryOperatorSymbols)
        {
            this.terminals.Add(TokenType.BinaryOperator, MakeLiteral(binaryOperatorSymbols));
        }

        private static Func<string, int, int?> MakeLiteral(params char[] literals)
        {
            return (source, offset) =>
            {
                foreach (var literal in literals)
                {
                    if (offset < source.Length && source[offset] == literal)
                    {
                        return 1;
                    }
                }

                return null;
            };
        }

        private static Func<string, int, int?> MakeLiteral(params string[] literals)
        {
            return (source, offset) =>
            {
                foreach (var literal in literals)
                {
                    if (offset < source.Length && source.Length >= offset + literal.Length && source.Substring(offset, literal.Length) == literal)
                    {
                        return literal.Length;
                    }
                }

                return null;
            };
        }

        private static Func<string, int, int?> MakeRegex(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled);

            return (source, offset) =>
            {
                var match = regex.Match(source, offset);
                if (match.Success && match.Index == offset)
                {
                    return match.Length;
                }

                return null;
            };
        }

        private int Advance(string source, Span span)
        {
            var offset = span.Start + span.Length;

            while (offset < span.Length && char.IsWhiteSpace(source, offset))
            {
                offset++;
            }

            return offset;
        }

        private Token Accept(TokenType tokenType, Cursor cursor)
        {
            var terminal = this.terminals[tokenType];
            var length = terminal(cursor.Source, cursor.Offset);
            if (length == null)
            {
                return null;
            }

            var span = new Span(cursor.Offset, length.Value);
            cursor.Offset = this.Advance(cursor.Source, span);

            return new Token(tokenType, cursor.Source, span);
        }

        private Token Expect(TokenType tokenType, Cursor cursor)
        {
            var token = this.Accept(tokenType, cursor);
            if (token == null)
            {
                throw new ParseException(cursor.Offset, tokenType);
            }

            return token;
        }

        public IEnumerable<Token> Scan(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var cursor = new Cursor(source);
            cursor.Offset = this.Advance(source, new Span(0, 0));

            foreach (var token in ScanExpression(cursor))
            {
                yield return token;
            }

            yield return Expect(TokenType.EOF, cursor);
        }

        private IEnumerable<Token> ScanExpression(Cursor cursor)
        {
            foreach (var token in ScanAtomicExpression(cursor))
            {
                yield return token;
            }

            Token binaryOp;
            while ((binaryOp = this.Accept(TokenType.BinaryOperator, cursor)) != null)
            {
                yield return binaryOp;

                foreach (var token in ScanAtomicExpression(cursor))
                {
                    yield return token;
                }
            }
        }

        private IEnumerable<Token> ScanAtomicExpression(Cursor cursor)
        {
            var variable = this.Accept(TokenType.Variable, cursor);
            if (variable != null)
            {
                yield return variable;
                yield break;
            }

            var unaryOp = this.Accept(TokenType.UnaryOperator, cursor);
            if (unaryOp != null)
            {
                yield return unaryOp;
                foreach (var token in ScanAtomicExpression(cursor))
                {
                    yield return token;
                }
                yield break;
            }

            var leftParen = this.Accept(TokenType.LeftParenthesis, cursor);
            if (leftParen != null)
            {
                yield return leftParen;
                foreach (var token in this.ScanExpression(cursor))
                {
                    yield return token;
                }
                yield return this.Expect(TokenType.RightParenthesis, cursor);
                yield break;
            }

            throw new ParseException(cursor.Offset, TokenType.Variable, TokenType.UnaryOperator, TokenType.LeftParenthesis);
        }

        private class Cursor
        {
            private readonly string source;

            public Cursor(string source)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                this.source = source;
            }

            public string Source
            {
                get { return this.source; }
            }

            public int Offset { get; set; }
        }
    }
}
