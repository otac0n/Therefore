namespace Therefore.Engine.Parser.Nodes
{
    using System;
    using Therefore.Engine.Parser.OperatorTypes;

    public sealed class BinaryOperatorNode : ParseTreeNode
    {
        public ParseTreeNode Left { get; set; }

        public Token Operator { get; set; }

        public IBinaryOperatorType OperatorType { get; set; }

        public ParseTreeNode Right { get; set; }
    }
}
