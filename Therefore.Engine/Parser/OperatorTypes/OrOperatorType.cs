namespace Therefore.Engine.Parser.OperatorTypes
{
    using System;
    using Therefore.Engine.Expressions;

    public sealed class OrOperatorType : IBinaryOperatorType
    {
        private static readonly OrOperatorType instance = new OrOperatorType();

        static OrOperatorType()
        {
        }

        public static OrOperatorType Instance
        {
            get { return instance; }
        }

        public string Name
        {
            get { return "or"; }
        }

        public Expression Create(Expression left, Expression right)
        {
            return new OrExpression(left, right);
        }
    }
}
