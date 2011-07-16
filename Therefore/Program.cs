namespace Therefore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Therefore.Engine;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter an expression:");
            string source = Console.ReadLine();
            Console.WriteLine();

            Expression expression;
            var nameTable = new List<string>();

            try
            {
                var parseTree = Parser.Parse(source);
                expression = Compiler.Compile(parseTree, nameTable);
            }
            catch (ParseException ex)
            {
                WriteParseException(ex, source);
                return;
            }

            var values = Solver.Solve(expression, nameTable.Count);

            if (values == null)
            {
                Console.WriteLine("Contradiction");
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    Console.WriteLine("{0}\t= {1}", nameTable[i], values[i]);
                }
            }
        }

        private static void WriteParseException(ParseException ex, string source)
        {
            Console.WriteLine(ex.Message);
            Console.Write(source.Substring(0, ex.Offset));
            var prevColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(ex.Offset == source.Length ? " " : source.Substring(ex.Offset, 1));
            Console.BackgroundColor = prevColor;
            Console.Write(ex.Offset == source.Length ? "" : source.Substring(ex.Offset + 1));
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
