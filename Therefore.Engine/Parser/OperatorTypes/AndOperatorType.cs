namespace Therefore.Engine.Parser.OperatorTypes
{
    using System;
    using Therefore.Engine.Expressions;

    public sealed class AndOperatorType : IBinaryOperatorType
    {
        private static readonly AndOperatorType instance = new AndOperatorType();

        static AndOperatorType()
        {
        }

        public static AndOperatorType Instance
        {
            get { return instance; }
        }

        public string Name
        {
            get { return "and"; }
        }

        public Expression Create(Expression left, Expression right)
        {
            return new AndExpression(left, right);
        }
    }
}
