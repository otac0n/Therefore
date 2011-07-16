namespace Therefore.Engine.Parser.Nodes
{
    using System;

    public sealed class BinaryOperatorNode : ParseTreeNode
    {
        public ParseTreeNode Left { get; set; }

        public Token Operator { get; set; }

        public ParseTreeNode Right { get; set; }
    }
}
