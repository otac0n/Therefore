namespace Therefore.Engine.Parser
{
    using System;

    public enum TokenType
    {
        Variable,
        BinaryOperator,
        UnaryOperator,
        LeftParenthesis,
        RightParenthesis,
        EOF,
    }

    public sealed class Token
    {
        private readonly TokenType tokenType;
        private readonly Span span;

        public Token(TokenType tokenType, Span span)
        {
            this.tokenType = tokenType;

            if (span == null)
            {
                throw new ArgumentNullException("span");
            }

            this.span = span;
        }

        public TokenType TokenType
        {
            get { return this.tokenType; }
        }

        public Span Span
        {
            get { return this.span; }
        }
    }
}
