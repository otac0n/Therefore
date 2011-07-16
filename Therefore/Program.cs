namespace Therefore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter an expression:");
            string source = Console.ReadLine();

            try
            {
                var parseTree = Parser.Parse(source);
            }
            catch (ParseException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.Write(source.Substring(0, ex.Offset));
                var prevColor = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(ex.Offset == source.Length ? " " : source.Substring(ex.Offset, 1));
                Console.BackgroundColor = prevColor;
                Console.Write(ex.Offset == source.Length ? "" : source.Substring(ex.Offset + 1));
                Console.WriteLine();
                Console.WriteLine();
                return;
            }

            var expression = And(And(A, Not(B)), Or(C, Not(C)));

            var values = Solve(expression, 3);

            if (values == null)
            {
                Console.WriteLine("Contradiction");
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Console.WriteLine("{0} = {1}", (char)('A' + i), values[i]);
                }
            }
        }

        private static bool?[] Solve(Expression expression, int variableCount)
        {
            // Allocate space for the variables and solutions.
            var variables = new bool?[variableCount];
            var validSolutions = new List<bool?[]>();

            // Determine the number of permutations that needs to happen.
            var count = 1 << variableCount;

            // We will use an integer to do the bit permutations.
            for (int current = 0; current < count; current++)
            {
                // Copy the bits of the integer `current` into the variables array.
                for (int bit = 0; bit < variableCount; bit++)
                {
                    variables[bit] = (current & (1 << bit)) > 0;
                }

                // Evaluate the expression with the current variables.
                var result = expression.Evaluate(variables);

                // If the result is `true`, this is a valid solution.
                if (result == true)
                {
                    // Copy the variables into the list of valid solutions.
                    var varCopy = new bool?[variableCount];
                    Array.Copy(variables, varCopy, variableCount);
                    validSolutions.Add(varCopy);
                }
            }

            // If there are no valid solutions, there must be a contradiction.
            if (validSolutions.Count == 0)
            {
                return null;
            }

            // Allocate space for the results.
            var results = new bool?[variableCount];

            // Copy the first valid solution into the results array.
            Array.Copy(validSolutions[0], results, variableCount);

            // Compare the current results with each of the subsequent solutions.
            foreach (var solution in validSolutions.Skip(1))
            {
                // Compare each variable.
                for (int var = 0; var < variableCount; var++)
                {
                    // If the results of the solution is different from the current result, mark it as undetermined.
                    if (results[var] != solution[var])
                    {
                        results[var] = null;
                    }
                }
            }

            return results;
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
