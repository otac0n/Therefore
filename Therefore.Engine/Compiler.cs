namespace Therefore.Engine
{
    using System;
    using System.Collections.Generic;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;
    using Therefore.Engine.Parser.Nodes;

    public static class Compiler
    {
        public static Expression Compile(ParseTree parseTree, IList<string> names, IEqualityComparer<string> nameComparison = null)
        {
            return Compile(parseTree.RootNode, new NameTable(names, nameComparison));
        }

        private static Expression Compile(ParseTreeNode parseTreeNode, NameTable nameTable)
        {
            var binNode = parseTreeNode as BinaryOperatorNode;
            if (binNode != null)
            {
                return CompileBinaryOperator(binNode, nameTable);
            }

            var unNode = parseTreeNode as UnaryOperatorNode;
            if (unNode != null)
            {
                return CompileUnaryOperator(unNode, nameTable);
            }

            var parenNode = parseTreeNode as ParenthesisNode;
            if (parenNode != null)
            {
                return CompileParenthesisNode(parenNode, nameTable);
            }

            var varNode = parseTreeNode as VariableNode;
            if (varNode != null)
            {
                return CompileVariableNode(varNode, nameTable);
            }

            throw new NotImplementedException("Unknown node type '" + parseTreeNode.GetType().Name + "'.");
        }

        private static Expression CompileBinaryOperator(BinaryOperatorNode binNode, NameTable nameTable)
        {
            var left = Compile(binNode.Left, nameTable);

            if (binNode.Right == null)
            {
                return left;
            }

            var right = Compile(binNode.Right, nameTable);

            switch (binNode.Operator.Value)
            {
                case "&":
                    return new AndExpression(left, right);
                case "|":
                    return new OrExpression(left, right);
                case ">":
                    return new ThenExpression(left, right);
            }

            throw new NotImplementedException("Unknown binary operator '" + binNode.Operator.Value + "'.");
        }

        private static Expression CompileUnaryOperator(UnaryOperatorNode unNode, NameTable nameTable)
        {
            var operand = Compile(unNode.Operand, nameTable);

            switch (unNode.Operator.Value)
            {
                case "~":
                    return new NotExpression(operand);
            }

            throw new NotImplementedException("Unknown unary operator '" + unNode.Operator.Value + "'.");
        }

        private static Expression CompileParenthesisNode(ParenthesisNode parenNode, NameTable nameTable)
        {
            return Compile(parenNode.Contained, nameTable);
        }

        private static Expression CompileVariableNode(VariableNode varNode, NameTable nameTable)
        {
            string name = varNode.Variable.Value;
            var index = nameTable.GetName(name);
            return new VariableExpression(index);
        }

        private class NameTable
        {
            private readonly IList<string> names;
            private readonly IEqualityComparer<string> comparison;

            public NameTable(IList<string> names, IEqualityComparer<string> comparison)
            {
                if (names == null)
                {
                    throw new ArgumentNullException("names");
                }

                this.names = names;

                if (comparison == null)
                {
                    comparison = EqualityComparer<string>.Default;
                }

                this.comparison = comparison;
            }

            public int GetName(string name)
            {
                var count = names.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this.comparison.Equals(names[i], name))
                    {
                        return i;
                    }
                }

                names.Add(name);
                return count;
            }
        }
    }
}
