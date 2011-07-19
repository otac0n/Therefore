namespace Therefore.Engine.Compiler.Constraints
{
    using System;
    using Therefore.Engine.Parser.Nodes;

    public sealed class ParenthesizedNotConstraint : Constraint
    {
        public override ConstraintViolation VisitBinaryOperatorNode(BinaryOperatorNode binNode)
        {
            return null;
        }

        public override ConstraintViolation VisitUnaryOperatorNode(UnaryOperatorNode unNode)
        {
            if (unNode.Operator.Value == "~")
            {
                var operand = unNode.Operand as UnaryOperatorNode;
                if (operand != null && operand.Operator.Value == "~")
                {
                    return new ConstraintViolation(unNode, "The 'not' operator may not be applied directly to the result of another 'not' operator.");
                }
            }

            return null;
        }

        public override ConstraintViolation VisitParenthesisNode(ParenthesisNode parenNode)
        {
            return null;
        }

        public override ConstraintViolation VisitVariableNode(VariableNode varNode)
        {
            return null;
        }
    }
}
