namespace Therefore.Engine.Parser
{
    using System;

    public sealed class Span
    {
        private readonly int start;
        private readonly int length;

        public Span(int start, int length)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start");
            }

            this.start = start;

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            this.length = length;
        }

        public int Start
        {
            get { return this.start; }
        }

        public int Length
        {
            get { return this.length; }
        }
    }
}
