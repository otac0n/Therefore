namespace Therefore.Engine.Parser.OperatorTypes
{
    using System;
    using Therefore.Engine.Expressions;

    public sealed class ThenOperatorType : IBinaryOperatorType
    {
        private static readonly ThenOperatorType instance = new ThenOperatorType();

        static ThenOperatorType()
        {
        }

        public static ThenOperatorType Instance
        {
            get { return instance; }
        }

        public string Name
        {
            get { return "then"; }
        }

        public Expression Create(Expression left, Expression right)
        {
            return new ThenExpression(left, right);
        }
    }
}
