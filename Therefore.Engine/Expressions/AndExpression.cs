namespace Therefore.Engine.Expressions
{
    using System;

    public sealed class AndExpression : BinaryExpression
    {
        public AndExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public override bool? Evaluate(bool?[] variables)
        {
            var leftResult = this.left.Evaluate(variables);

            if (leftResult == false)
            {
                return false;
            }

            return leftResult & this.right.Evaluate(variables);
        }

        public override string ToString()
        {
            return string.Format("And({0}, {1})", this.left, this.right);
        }
    }
}
