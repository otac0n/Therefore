namespace Therefore.Engine.Compiler.Constraints
{
    using System;
    using System.Collections.Generic;
    using Therefore.Engine.Parser;
    using Therefore.Engine.Parser.Nodes;

    public sealed class ConstraintVisior
    {
        private readonly Constraint constraint;

        public ConstraintVisior(Constraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            this.constraint = constraint;
        }

        public IEnumerable<ConstraintViolation> Visit(ParseTree parseTree)
        {
            return VisitNode(parseTree.RootNode);
        }

        private IEnumerable<ConstraintViolation> VisitNode(ParseTreeNode parseTreeNode)
        {
            var binNode = parseTreeNode as BinaryOperatorNode;
            if (binNode != null)
            {
                return this.VisitBinaryOperatorNode(binNode);
            }

            var unNode = parseTreeNode as UnaryOperatorNode;
            if (unNode != null)
            {
                return this.VisitUnaryOperatorNode(unNode);
            }

            var parenNode = parseTreeNode as ParenthesisNode;
            if (parenNode != null)
            {
                return this.VisitParenthesisNode(parenNode);
            }

            var varNode = parseTreeNode as VariableNode;
            if (varNode != null)
            {
                return this.VisitVariableNode(varNode);
            }

            throw new NotImplementedException("Unknown node type '" + parseTreeNode.GetType().Name + "'.");
        }

        private IEnumerable<ConstraintViolation> VisitBinaryOperatorNode(BinaryOperatorNode binNode)
        {
            var myViolation = this.constraint.VisitBinaryOperatorNode(binNode);
            if (myViolation != null)
            {
                yield return myViolation;
            }

            foreach (var violation in this.VisitNode(binNode.Left))
            {
                yield return violation;
            }

            foreach (var violation in this.VisitNode(binNode.Right))
            {
                yield return violation;
            }
        }

        private IEnumerable<ConstraintViolation> VisitUnaryOperatorNode(UnaryOperatorNode unNode)
        {
            var myViolation = this.constraint.VisitUnaryOperatorNode(unNode);
            if (myViolation != null)
            {
                yield return myViolation;
            }

            foreach (var violation in this.VisitNode(unNode.Operand))
            {
                yield return violation;
            }
        }

        private IEnumerable<ConstraintViolation> VisitParenthesisNode(ParenthesisNode parenNode)
        {
            var myViolation = this.constraint.VisitParenthesisNode(parenNode);
            if (myViolation != null)
            {
                yield return myViolation;
            }

            foreach (var violation in this.VisitNode(parenNode.Contained))
            {
                yield return violation;
            }
        }

        private IEnumerable<ConstraintViolation> VisitVariableNode(VariableNode varNode)
        {
            var myViolation = this.constraint.VisitVariableNode(varNode);
            if (myViolation != null)
            {
                yield return myViolation;
            }
        }
    }
}
