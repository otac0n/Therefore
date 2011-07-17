namespace Therefore.Engine.Compiler.Constraints
{
    using System;
    using Therefore.Engine.Parser.Nodes;

    public abstract class ConstraintViolation
    {
        private readonly ParseTreeNode violatingNode;
        private readonly string message;

        public ConstraintViolation(ParseTreeNode violatingNode, string message)
        {
            if (violatingNode == null)
            {
                throw new ArgumentNullException("violatingNode");
            }

            this.violatingNode = violatingNode;

            this.message = message ?? string.Empty;
        }

        public ParseTreeNode ViolatingNode
        {
            get { return this.violatingNode; }
        }

        public string Message
        {
            get { return this.message; }
        }
    }
}
