namespace Therefore.Engine.Game
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
                    return new ParenthesizedNotConstraintViolation(unNode);
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

        public sealed class ParenthesizedNotConstraintViolation : ConstraintViolation
        {
            public ParenthesizedNotConstraintViolation(ParseTreeNode violatingNode)
                : base(violatingNode, "The 'not' operator may not be applied directly to the result of another 'not' operator.")
            {

            }
        }
    }
}
