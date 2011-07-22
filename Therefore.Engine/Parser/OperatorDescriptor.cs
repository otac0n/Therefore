namespace Therefore.Engine.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Therefore.Engine.Parser.OperatorTypes;

    public sealed class OperatorDescriptor
    {
        private readonly IBinaryOperatorType binaryOperatorType;
        private readonly OperatorAssociativity associativity;
        private readonly IList<string> operatorSymbols;

        public OperatorDescriptor(IBinaryOperatorType binaryOperatorType, OperatorAssociativity associativity, IEnumerable<string> operatorSymbols)
        {
            if (binaryOperatorType == null)
            {
                throw new ArgumentNullException("binaryOperatorType");
            }

            this.binaryOperatorType = binaryOperatorType;

            this.associativity = associativity;

            if (operatorSymbols == null)
            {
                throw new ArgumentNullException("operatorSymbols");
            }

            this.operatorSymbols = operatorSymbols.ToList().AsReadOnly();
        }

        public IBinaryOperatorType BinaryOperatorType
        {
            get
            {
                return this.binaryOperatorType;
            }
        }

        public OperatorAssociativity Associativity
        {
            get
            {
                return this.associativity;
            }
        }

        public IList<string> OperatorSymbols
        {
            get
            {
                return this.operatorSymbols;
            }
        }
    }
}
