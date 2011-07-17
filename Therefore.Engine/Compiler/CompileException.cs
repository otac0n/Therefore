namespace Therefore.Engine.Compiler
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Therefore.Engine.Parser.Nodes;

    /// <summary>
    /// Thrown when there is an error compiling.
    /// </summary>
    [Serializable]
    public class CompileException : Exception
    {
        private readonly ParseTreeNode node;

        /// <summary>
        /// Initializes a new instance of the CompileException class.
        /// </summary>
        public CompileException(ParseTreeNode node, string message)
            : this(message)
        {
            this.node = node;
        }

        /// <summary>
        /// Initializes a new instance of the CompileException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CompileException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CompileException class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CompileException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CompileException class with serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The class name is null or System.Exception.HResult is zero (0).</exception>
        protected CompileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ParseTreeNode Node
        {
            get { return this.node; }
        }
    }
}
