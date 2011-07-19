namespace Therefore.Engine.Parser.Nodes
{
    using System;

    public class ParseTreeNode
    {
        public string NodeType
        {
            get
            {
                return this.GetType().Name;
            }
        }
    }
}
