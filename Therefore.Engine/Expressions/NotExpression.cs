namespace Therefore.Engine.Expressions
{
    using System;

    public sealed class NotExpression : Expression
    {
        private readonly Expression operand;

        public NotExpression(Expression operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }

            this.operand = operand;
        }

        public Expression Operand
        {
            get { return this.operand; }
        }

        public override bool? Evaluate(bool?[] variables)
        {
            var result = this.operand.Evaluate(variables);

            return !result;
        }
    }
}
