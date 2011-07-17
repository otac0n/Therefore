namespace Therefore.Engine.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Therefore.Engine.Expressions;

    public static class Scanner
    {
        private readonly static Dictionary<TokenType, Func<string, int, int?>> terminals = new Dictionary<TokenType, Func<string, int, int?>>
        {
            { TokenType.Variable, MakeRegex(@"\w+") },
            { TokenType.LeftParenthesis, MakeLiteral('(') },
            { TokenType.RightParenthesis, MakeLiteral(')') },
            { TokenType.UnaryOperator, MakeLiteral('~') },
            { TokenType.BinaryOperator, MakeLiteral('&', '|', '>') },
            { TokenType.EOF, (source, location) =>
                {
                    return location == source.Length ? (int?)0 : null;
                }
            },
        };

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

        private static int Advance(Span span)
        {
            var source = span.Source;
            var offset = span.Start + span.Length;

            while (offset < span.Length && char.IsWhiteSpace(source, offset))
            {
                offset++;
            }

            return offset;
        }

        private static Token Accept(TokenType tokenType, Cursor cursor)
        {
            var terminal = terminals[tokenType];
            var length = terminal(cursor.Source, cursor.Offset);
            if (length == null)
            {
                return null;
            }

            var span = new Span(cursor.Source, cursor.Offset, length.Value);
            cursor.Offset = Advance(span);

            return new Token(tokenType, span);
        }

        private static Token Expect(TokenType tokenType, Cursor cursor)
        {
            var token = Accept(tokenType, cursor);
            if (token == null)
            {
                throw new ParseException(cursor.Offset, tokenType);
            }

            return token;
        }

        public static IEnumerable<Token> Scan(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var cursor = new Cursor(source);
            cursor.Offset = Advance(new Span(source, 0, 0));

            foreach (var token in ScanExpression(cursor))
            {
                yield return token;
            }

            yield return Expect(TokenType.EOF, cursor);
        }

        private static IEnumerable<Token> ScanExpression(Cursor cursor)
        {
            foreach (var token in ScanAtomicExpression(cursor))
            {
                yield return token;
            }

            Token binaryOp;
            while ((binaryOp = Accept(TokenType.BinaryOperator, cursor)) != null)
            {
                yield return binaryOp;

                foreach (var token in ScanAtomicExpression(cursor))
                {
                    yield return token;
                }
            }
        }

        private static IEnumerable<Token> ScanAtomicExpression(Cursor cursor)
        {
            var variable = Accept(TokenType.Variable, cursor);
            if (variable != null)
            {
                yield return variable;
                yield break;
            }

            var unaryOp = Accept(TokenType.UnaryOperator, cursor);
            if (unaryOp != null)
            {
                yield return unaryOp;
                foreach (var token in ScanAtomicExpression(cursor))
                {
                    yield return token;
                }
                yield break;
            }

            var leftParen = Accept(TokenType.LeftParenthesis, cursor);
            if (leftParen != null)
            {
                yield return leftParen;
                foreach (var token in ScanExpression(cursor))
                {
                    yield return token;
                }
                yield return Expect(TokenType.RightParenthesis, cursor);
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
