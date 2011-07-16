namespace Therefore.Engine.Parser
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when there is an error parsing.
    /// </summary>
    [Serializable]
    public class ParseException : Exception
    {
        private readonly int offset;

        /// <summary>
        /// Initializes a new instance of the ParseException class.
        /// </summary>
        public ParseException(int offset, params TokenType[] tokenTypes)
            : this(FormatMessage(tokenTypes, offset), offset)
        {
        }

        private static string FormatMessage(TokenType[] tokenTypes, int offset)
        {
            if (tokenTypes.Length == 0)
            {
                return string.Format("Expected (unkonwn) at character {0}.", offset);
            }

            if (tokenTypes.Length == 1)
            {
                return string.Format("Expected {0} at character {1}.", tokenTypes[0], offset);
            }

            var tokenTypeList = string.Join(", ", tokenTypes.Take(tokenTypes.Length - 1).Select(t => t.ToString())) + " or " + tokenTypes[tokenTypes.Length - 1];
            return string.Format("Expected {0} at character {1}.", tokenTypeList, offset);
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class.
        /// </summary>
        public ParseException(string message, int offset)
            : this(message)
        {
            this.offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ParseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class with serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The class name is null or System.Exception.HResult is zero (0).</exception>
        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public int Offset
        {
            get { return this.offset; }
        }
    }
}
