namespace Therefore.Engine.Parser.Nodes
{
    using System;

    public sealed class UnaryOperatorNode : ParseTreeNode
    {
        public Token Operator { get; set; }

        public ParseTreeNode Operand { get; set; }
    }
}
