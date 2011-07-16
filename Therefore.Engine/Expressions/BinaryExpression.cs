namespace Therefore.Engine.Expressions
{
    using System;

    public abstract class BinaryExpression : Expression
    {
        protected readonly Expression left;
        protected readonly Expression right;

        protected BinaryExpression(Expression left, Expression right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            this.left = left;

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            this.right = right;
        }

        public Expression Left
        {
            get { return this.left; }
        }

        public Expression Right
        {
            get { return this.right; }
        }
    }
}
