namespace Therefore.Engine.Parser
{
    public sealed class ParseTree
    {
        public ParseTree(Nodes.ParseTreeNode rootNode)
        {
            this.RootNode = rootNode;
        }

        public Nodes.ParseTreeNode RootNode { get; private set; }
    }
}
