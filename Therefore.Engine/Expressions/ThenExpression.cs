namespace Therefore.Engine.Expressions
{
    using System;

    public sealed class ThenExpression : BinaryExpression
    {
        public ThenExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public override bool? Evaluate(bool?[] variables)
        {
            var leftResult = this.left.Evaluate(variables);

            if (leftResult == false)
            {
                return true;
            }

            return !leftResult | this.right.Evaluate(variables);
        }
    }
}
