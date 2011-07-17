namespace Therefore.Engine.Compiler
{
    using System;
    using System.Collections.Generic;
    using Therefore.Engine.Compiler.Constraints;

    public sealed class CompilerOptions
    {
        public IList<Constraint> Constraints { get; set; }
    }
}
