namespace Therefore.Engine.Parser.Nodes
{
    using System;

    public sealed class VariableNode : ParseTreeNode
    {
        public Token Variable { get; set; }
    }
}
