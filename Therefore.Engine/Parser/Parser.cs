namespace Therefore.Engine.Parser
{
    using System;
    using System.Collections.Generic;
    using Therefore.Engine.Expressions;

    public static class Parser
    {
        public static ParseTree Parse(string source)
        {
            var tokenStream = Scanner.Scan(source).GetEnumerator();
            tokenStream.MoveNext();

            var node = ParseAndExpression(tokenStream);

            if (tokenStream.Current.TokenType != TokenType.EOF)
            {
                throw ThrowHelper(tokenStream);
            }

            return new ParseTree(node);
        }

        private static Nodes.ParseTreeNode ParseAndExpression(IEnumerator<Token> tokenStream)
        {
            var left = ParseOrExpression(tokenStream);

            if (tokenStream.Current.TokenType == TokenType.BinaryOperator && tokenStream.Current.Value == "&")
            {
                var andNode = new Nodes.BinaryOperatorNode();
                andNode.Left = left;

                andNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                andNode.Right = ParseAndExpression(tokenStream);
                return andNode;
            }

            return left;
        }

        private static Nodes.ParseTreeNode ParseOrExpression(IEnumerator<Token> tokenStream)
        {
            var left = ParseThenExpression(tokenStream);

            if (tokenStream.Current.TokenType == TokenType.BinaryOperator && tokenStream.Current.Value == "|")
            {
                var orNode = new Nodes.BinaryOperatorNode();
                orNode.Left = left;

                orNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                orNode.Right = ParseOrExpression(tokenStream);
                return orNode;
            }

            return left;
        }

        private static Nodes.ParseTreeNode ParseThenExpression(IEnumerator<Token> tokenStream)
        {
            var left = ParseNotExpression(tokenStream);

            if (tokenStream.Current.TokenType == TokenType.BinaryOperator && tokenStream.Current.Value == ">")
            {
                var thenNode = new Nodes.BinaryOperatorNode();
                thenNode.Left = left;

                thenNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                thenNode.Right = ParseThenExpression(tokenStream);
                return thenNode;
            }

            return left;
        }

        private static Nodes.ParseTreeNode ParseNotExpression(IEnumerator<Token> tokenStream)
        {
            if (tokenStream.Current.TokenType == TokenType.UnaryOperator && tokenStream.Current.Value == "~")
            {
                var notNode = new Nodes.UnaryOperatorNode();

                notNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                notNode.Operand = ParseNotExpression(tokenStream);

                return notNode;
            }
            else
            {
                return ParseParenExpression(tokenStream);
            }
        }

        private static Nodes.ParseTreeNode ParseParenExpression(IEnumerator<Token> tokenStream)
        {
            if (tokenStream.Current.TokenType == TokenType.LeftParenthesis)
            {
                var parenNode = new Nodes.ParenthesisNode();

                parenNode.LeftParenthesis = tokenStream.Current;
                tokenStream.MoveNext();

                parenNode.Contained = ParseAndExpression(tokenStream);

                if (tokenStream.Current.TokenType != TokenType.RightParenthesis)
                {
                    throw ThrowHelper(tokenStream);
                }

                parenNode.RightParenthesis = tokenStream.Current;
                tokenStream.MoveNext();

                return parenNode;
            }
            else
            {
                return ParseVariable(tokenStream);
            }
        }

        private static Nodes.ParseTreeNode ParseVariable(IEnumerator<Token> tokenStream)
        {
            if (tokenStream.Current.TokenType == TokenType.Variable)
            {
                var varNode = new Nodes.VariableNode
                {
                    Variable = tokenStream.Current
                };

                tokenStream.MoveNext();
                return varNode;
            }
            else
            {
                throw ThrowHelper(tokenStream);
            }
        }

        private static Exception ThrowHelper(IEnumerator<Token> tokenStream)
        {
            throw new ParseException("Unexpected " + tokenStream.Current.TokenType + " at " + tokenStream.Current.Span.Start, tokenStream.Current.Span.Start);
        }
    }
}
