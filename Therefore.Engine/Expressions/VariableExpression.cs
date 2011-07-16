namespace Therefore.Engine.Expressions
{
    using System;

    public sealed class VariableExpression : Expression
    {
        private readonly int variable;

        public VariableExpression(int variable)
        {
            if (variable < 0)
            {
                throw new ArgumentOutOfRangeException("variable");
            }

            this.variable = variable;
        }

        public int Variable
        {
            get { return this.variable; }
        }

        public override bool? Evaluate(bool?[] variables)
        {
            if (this.variable >= variables.Length)
            {
                throw new InvalidOperationException("Parameter index {0} does not exist in the parameter list.");
            }

            return variables[this.variable];
        }

        public override string ToString()
        {
            return "$" + this.variable;
        }
    }
}
