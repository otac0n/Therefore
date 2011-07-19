namespace Therefore.Engine.Parser
{
    using System;

    public sealed class ParseTree
    {
        private readonly string source;

        public ParseTree(string source, Nodes.ParseTreeNode rootNode)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.source = source;

            this.RootNode = rootNode;
        }

        public Nodes.ParseTreeNode RootNode { get; private set; }

        public string Source
        {
            get
            {
                return this.source;
            }
        }
    }
}
