namespace Therefore.Engine.Parser
{
    using System;

    public sealed class Span
    {
        private readonly string source;
        private readonly int start;
        private readonly int length;

        public Span(string source, int start, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.source = source;

            if (start < 0 || start > source.Length)
            {
                throw new ArgumentOutOfRangeException("start");
            }

            this.start = start;

            if (length < 0 || (start + length) > source.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            this.length = length;
        }

        public string Source
        {
            get { return this.source; }
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
