using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Therefore.Engine.Parser.OperatorTypes;

namespace Therefore.Engine.Parser
{
    public class OperatorDescriptorList : List<OperatorDescriptor>
    {
        public void Add(IBinaryOperatorType binaryOperatorType, OperatorAssociativity associativity, params string[] operatorSymbols)
        {
            this.Add(new OperatorDescriptor(binaryOperatorType, associativity, operatorSymbols));
        }
    }
}
