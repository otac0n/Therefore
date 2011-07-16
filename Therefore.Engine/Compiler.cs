namespace Therefore.Engine
{
    using System;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;
    using Therefore.Engine.Parser.Nodes;
    using System.Collections.Generic;

    public static class Compiler
    {
        public static Expression Compile(ParseTree parseTree, IList<string> nameTable)
        {
            return Compile(parseTree.RootNode, nameTable);
        }

        private static Expression Compile(ParseTreeNode parseTreeNode, IList<string> nameTable)
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

        private static Expression CompileBinaryOperator(BinaryOperatorNode binNode, IList<string> nameTable)
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

        private static Expression CompileUnaryOperator(UnaryOperatorNode unNode, IList<string> nameTable)
        {
            var operand = Compile(unNode.Operand, nameTable);

            switch (unNode.Operator.Value)
            {
                case "~":
                    return new NotExpression(operand);
            }

            throw new NotImplementedException("Unknown unary operator '" + unNode.Operator.Value + "'.");
        }

        private static Expression CompileParenthesisNode(ParenthesisNode parenNode, IList<string> nameTable)
        {
            return Compile(parenNode.Contained, nameTable);
        }

        private static Expression CompileVariableNode(VariableNode varNode, IList<string> nameTable)
        {
            string name = varNode.Variable.Value;

            var index = nameTable.IndexOf(name);
            if (index == -1)
            {
                index = nameTable.Count;
                nameTable.Add(name);
            }

            return new VariableExpression(index);
        }
    }
}
