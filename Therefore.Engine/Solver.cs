namespace Therefore.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Therefore.Engine.Expressions;

    public static class Solver
    {
        public static bool?[] Solve(Expression expression, int variableCount)
        {
            // Allocate space for the variables and solutions.
            var variables = new bool?[variableCount];
            var validSolutions = new List<bool?[]>();

            // Determine the number of permutations that need to happen.
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
    }
}
