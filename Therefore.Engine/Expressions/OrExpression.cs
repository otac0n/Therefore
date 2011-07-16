namespace Therefore.Engine.Expressions
{
    using System;

    public sealed class OrExpression : BinaryExpression
    {
        public OrExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public override bool? Evaluate(bool?[] variables)
        {
            var leftResult = this.left.Evaluate(variables);

            if (leftResult == true)
            {
                return true;
            }

            return leftResult | this.right.Evaluate(variables);
        }

        public override string ToString()
        {
            return string.Format("Or({0}, {1})", this.left, this.right);
        }
    }
}
