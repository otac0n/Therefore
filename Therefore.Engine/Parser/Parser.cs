namespace Therefore.Engine.Parser
{
    using System;
    using System.Collections.Generic;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser.OperatorTypes;

    public static class Parser
    {
        private static OperatorDescriptorList binaryOperators = new OperatorDescriptorList()
        {
            { AndOperatorType.Instance, OperatorAssociativity.LeftAssociative, "&", "∧", "·" },
            { OrOperatorType.Instance, OperatorAssociativity.LeftAssociative, "|", "∨", "+" },
            { ThenOperatorType.Instance, OperatorAssociativity.LeftAssociative, ">", "->", "→", "⇒", "⊃" },
        };

        public static ParseTree Parse(string source)
        {
            var tokenStream = Scanner.Scan(source).GetEnumerator();
            tokenStream.MoveNext();

            var node = ParseBinaryExpression(tokenStream, 0);

            if (tokenStream.Current.TokenType != TokenType.EOF)
            {
                throw ThrowHelper(tokenStream);
            }

            return new ParseTree(source, node);
        }

        private static Nodes.ParseTreeNode ParseBinaryExpression(IEnumerator<Token> tokenStream, int binaryOperatorIndex)
        {
            if (binaryOperatorIndex == binaryOperators.Count)
            {
                return ParseNotExpression(tokenStream);
            }

            var binaryOperator = binaryOperators[binaryOperatorIndex];

            var operands = new List<Nodes.ParseTreeNode>();
            var operators = new List<Token>();

            operands.Add(ParseBinaryExpression(tokenStream, binaryOperatorIndex + 1));

            while (tokenStream.Current.TokenType == TokenType.BinaryOperator && binaryOperator.OperatorSymbols.Contains(tokenStream.Current.Value))
            {
                operators.Add(tokenStream.Current);
                tokenStream.MoveNext();

                operands.Add(ParseBinaryExpression(tokenStream, binaryOperatorIndex + 1));
            }

            return CombineBinaryOperators(operands, operators, binaryOperator.BinaryOperatorType, binaryOperator.Associativity);
        }

        private static Nodes.ParseTreeNode CombineBinaryOperators(List<Nodes.ParseTreeNode> operands, List<Token> operators, IBinaryOperatorType operatorType, OperatorAssociativity associativity)
        {
            if (operators.Count + 1 != operands.Count)
            {
                throw new InvalidOperationException("You cannot combine binary operators if the number of operands is not equal to the number of operators plus one.");
            }

            // If there is just one operand, just return it.
            if (operands.Count == 1)
            {
                return operands[0];
            }

            // If there are just two operands, all associativity rules are equivalent.
            if (operands.Count == 2)
            {
                return new Nodes.BinaryOperatorNode
                {
                    Left = operands[0],
                    Operator = operators[0],
                    OperatorType = operatorType,
                    Right = operands[1],
                };
            }

            // If there are more than two operands and the operator is non-associative, throw a parse error.
            if (associativity == OperatorAssociativity.NonAssociative)
            {
                throw new ParseException("The operator '" + operatorType.Name + "' is non-associative and therefore may not be used in groups of 3 or more without using parentheses for clarification.", operators[2].Span.Start);
            }

            // Otherwise, build up the tree with the proper associativity.
            if (associativity == OperatorAssociativity.LeftAssociative)
            {
                Nodes.ParseTreeNode accumulator = null;
                for (int i = 0; i < operands.Count; i++)
                {
                    if (accumulator == null)
                    {
                        accumulator = operands[i];
                    }
                    else
                    {
                        var thenNode = new Nodes.BinaryOperatorNode
                        {
                            Left = accumulator,
                            Operator = operators[i - 1],
                            OperatorType = operatorType,
                            Right = operands[i],
                        };

                        accumulator = thenNode;
                    }
                }

                return accumulator;
            }
            else
            {
                Nodes.ParseTreeNode accumulator = null;
                for (int i = operands.Count - 1; i >= 0; i--)
                {
                    if (accumulator == null)
                    {
                        accumulator = operands[i];
                    }
                    else
                    {
                        var thenNode = new Nodes.BinaryOperatorNode
                        {
                            Left = operands[i],
                            Operator = operators[i],
                            OperatorType = operatorType,
                            Right = accumulator
                        };

                        accumulator = thenNode;
                    }
                }

                return accumulator;
            }
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

                parenNode.Contained = ParseBinaryExpression(tokenStream, 0);

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
