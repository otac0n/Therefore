namespace Therefore.Engine.Game
{
    using System;
    using Therefore.Engine.Parser.Nodes;

    public abstract class Constraint
    {
        public abstract ConstraintViolation VisitBinaryOperatorNode(BinaryOperatorNode binNode);

        public abstract ConstraintViolation VisitUnaryOperatorNode(UnaryOperatorNode unNode);

        public abstract ConstraintViolation VisitParenthesisNode(ParenthesisNode parenNode);

        public abstract ConstraintViolation VisitVariableNode(VariableNode varNode);
    }
}
