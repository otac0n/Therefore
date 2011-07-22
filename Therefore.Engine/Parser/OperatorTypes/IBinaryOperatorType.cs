namespace Therefore.Engine.Parser.OperatorTypes
{
    using System;
    using Therefore.Engine.Expressions;

    public interface IBinaryOperatorType
    {
        string Name { get; }

        Expression Create(Expression left, Expression right);
    }
}
