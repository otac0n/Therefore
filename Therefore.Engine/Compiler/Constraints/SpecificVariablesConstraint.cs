namespace Therefore.Engine.Compiler.Constraints
{
    using System;
    using System.Linq;
    using Therefore.Engine.Parser.Nodes;
    using System.Collections.Generic;

    public sealed class SpecificVariablesConstraint : Constraint
    {
        private readonly string[] variables;
        private readonly IEqualityComparer<string> comparison;

        public SpecificVariablesConstraint(string[] variables, IEqualityComparer<string> comparison = null)
        {
            if (variables == null)
            {
                throw new ArgumentNullException("variables");
            }

            this.variables = variables.ToArray();

            this.comparison = comparison ?? EqualityComparer<string>.Default;
        }

        public override ConstraintViolation VisitBinaryOperatorNode(BinaryOperatorNode binNode)
        {
            return null;
        }

        public override ConstraintViolation VisitUnaryOperatorNode(UnaryOperatorNode unNode)
        {
            return null;
        }

        public override ConstraintViolation VisitParenthesisNode(ParenthesisNode parenNode)
        {
            return null;
        }

        public override ConstraintViolation VisitVariableNode(VariableNode varNode)
        {
            var value = varNode.Variable.Value;

            if (!this.variables.Any(v => this.comparison.Equals(value, v)))
            {
                return new ConstraintViolation(varNode, "The variable name '" + value + "' is not allowed.");
            }

            return null;
        }
    }
}
