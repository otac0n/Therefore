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
            var andNode = new Nodes.BinaryOperatorNode();

            andNode.Left = ParseOrExpression(tokenStream);

            if (tokenStream.Current.TokenType == TokenType.BinaryOperator && tokenStream.Current.Value == "&")
            {
                andNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                andNode.Right = ParseAndExpression(tokenStream);
            }

            return andNode;
        }

        private static Nodes.ParseTreeNode ParseOrExpression(IEnumerator<Token> tokenStream)
        {
            var orNode = new Nodes.BinaryOperatorNode();

            orNode.Left = ParseThenExpression(tokenStream);

            if (tokenStream.Current.TokenType == TokenType.BinaryOperator && tokenStream.Current.Value == "|")
            {
                orNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                orNode.Right = ParseOrExpression(tokenStream);
            }

            return orNode;
        }

        private static Nodes.ParseTreeNode ParseThenExpression(IEnumerator<Token> tokenStream)
        {
            var thenNode = new Nodes.BinaryOperatorNode();

            thenNode.Left = ParseNotExpression(tokenStream);

            if (tokenStream.Current.TokenType == TokenType.BinaryOperator && tokenStream.Current.Value == ">")
            {
                thenNode.Operator = tokenStream.Current;
                tokenStream.MoveNext();

                thenNode.Right = ParseThenExpression(tokenStream);
            }

            return thenNode;
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
