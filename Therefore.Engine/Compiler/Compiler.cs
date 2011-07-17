namespace Therefore.Engine.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Therefore.Engine.Compiler.Constraints;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;
    using Therefore.Engine.Parser.Nodes;

    public sealed class Compiler
    {
        private readonly Constraint[] constraints;

        private static readonly CompilerOptions defaultOptions = new CompilerOptions
        {
            Constraints = new[] { new ParenthesizedNotConstraint() },
        };

        public Compiler(CompilerOptions options = null)
        {
            options = options ?? defaultOptions;

            this.constraints = options.Constraints.ToArray();
        }

        private static void CheckConstraints<T>(IEnumerable<Constraint> constraints, T node, Func<Constraint, T, ConstraintViolation> constraintCheck) where T : ParseTreeNode
        {
            foreach (var constraint in constraints)
            {
                var violation = constraintCheck(constraint, node);
                if (violation != null)
                {
                    throw new CompileException(violation.ViolatingNode, violation.Message);
                }
            }
        }

        public Expression Compile(ParseTree parseTree, IList<string> names, IEqualityComparer<string> nameComparison = null)
        {
            return this.Compile(parseTree.RootNode, new NameTable(names, nameComparison));
        }

        private Expression Compile(ParseTreeNode parseTreeNode, NameTable nameTable)
        {
            var binNode = parseTreeNode as BinaryOperatorNode;
            if (binNode != null)
            {
                return this.CompileBinaryOperator(binNode, nameTable);
            }

            var unNode = parseTreeNode as UnaryOperatorNode;
            if (unNode != null)
            {
                return this.CompileUnaryOperator(unNode, nameTable);
            }

            var parenNode = parseTreeNode as ParenthesisNode;
            if (parenNode != null)
            {
                return this.CompileParenthesisNode(parenNode, nameTable);
            }

            var varNode = parseTreeNode as VariableNode;
            if (varNode != null)
            {
                return this.CompileVariableNode(varNode, nameTable);
            }

            throw new CompileException(parseTreeNode, "Unknown node type '" + parseTreeNode.GetType().Name + "'.");
        }

        private Expression CompileBinaryOperator(BinaryOperatorNode binNode, NameTable nameTable)
        {
            CheckConstraints(this.constraints, binNode, (c, n) => c.VisitBinaryOperatorNode(n));

            var left = this.Compile(binNode.Left, nameTable);

            if (binNode.Right == null)
            {
                return left;
            }

            var right = this.Compile(binNode.Right, nameTable);

            switch (binNode.Operator.Value)
            {
                case "&":
                    return new AndExpression(left, right);
                case "|":
                    return new OrExpression(left, right);
                case ">":
                    return new ThenExpression(left, right);
            }

            throw new CompileException(binNode, "Unknown binary operator '" + binNode.Operator.Value + "'.");
        }

        private Expression CompileUnaryOperator(UnaryOperatorNode unNode, NameTable nameTable)
        {
            CheckConstraints(this.constraints, unNode, (c, n) => c.VisitUnaryOperatorNode(n));

            var operand = this.Compile(unNode.Operand, nameTable);

            switch (unNode.Operator.Value)
            {
                case "~":
                    return new NotExpression(operand);
            }

            throw new CompileException(unNode, "Unknown unary operator '" + unNode.Operator.Value + "'.");
        }

        private Expression CompileParenthesisNode(ParenthesisNode parenNode, NameTable nameTable)
        {
            CheckConstraints(this.constraints, parenNode, (c, n) => c.VisitParenthesisNode(n));

            return this.Compile(parenNode.Contained, nameTable);
        }

        private Expression CompileVariableNode(VariableNode varNode, NameTable nameTable)
        {
            CheckConstraints(this.constraints, varNode, (c, n) => c.VisitVariableNode(n));

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
