namespace Therefore.Engine.Expressions
{
    using System;

    public abstract class Expression
    {
        public abstract bool? Evaluate(bool?[] variables);
    }
}
