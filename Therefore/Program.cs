namespace Therefore
{
    using System;
    using Therefore.Engine.Expressions;

    class Program
    {
        static void Main(string[] args)
        {
            var expression = And(And(A, Not(B)), Or(C, Not(C)));
        }

        public static AndExpression And(Expression left, Expression right)
        {
            return new AndExpression(left, right);
        }

        public static OrExpression Or(Expression left, Expression right)
        {
            return new OrExpression(left, right);
        }

        public static NotExpression Not(Expression operand)
        {
            return new NotExpression(operand);
        }

        public static VariableExpression A
        {
            get
            {
                return new VariableExpression(0);
            }
        }

        public static VariableExpression B
        {
            get
            {
                return new VariableExpression(1);
            }
        }

        public static VariableExpression C
        {
            get
            {
                return new VariableExpression(2);
            }
        }

        public static VariableExpression D
        {
            get
            {
                return new VariableExpression(3);
            }
        }
    }
}
