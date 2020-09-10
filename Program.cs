using System;
using Newtonsoft.Json;

namespace Fluent.Expression.Evaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var exp1 = new ExpEval<float>();
                exp1.Build().GTE(0).LTE(50);


                var exp2 = new ExpEval<float>();
                //var expression = ExpressionBuilder.Build<float>().GTE(50).LTE(100);
                //exp2.ExpressionTree = expression;
                //or use the following code for inferring type implicitly.
                exp2.Build().MUL(3).LTE(100 * 2);

                //Now exp1 evaluates both exp1 & exp2 with AND
                exp1.AddInnerExpression(exp2);
                //Console.WriteLine(JsonConvert.SerializeObject(ac, Formatting.Indented) + "\n\n" + ac.Title);

                Console.WriteLine("Enter a test float value: ");
                var input = Console.ReadLine();
                bool run = true;
                while (run)
                {
                    switch (exp1.Evaluate(input))
                    {
                        case ExpEvalResultType.FALSE:
                            Console.WriteLine("Result: FALSE");
                            run = false;
                            break;
                        case ExpEvalResultType.INVALID:
                            Console.WriteLine("Invalid input!");
                            input = Console.ReadLine();
                            break;
                        case ExpEvalResultType.TRUE:
                            Console.WriteLine("Result: TRUE");
                            run = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
