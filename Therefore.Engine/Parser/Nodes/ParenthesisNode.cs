namespace Therefore.Engine.Parser.Nodes
{
    using System;

    public sealed class ParenthesisNode : ParseTreeNode
    {
        public Token LeftParenthesis { get; set; }

        public ParseTreeNode Contained { get; set; }

        public Token RightParenthesis { get; set; }
    }
}
