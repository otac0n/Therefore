namespace Therefore.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Therefore.Engine;
    using Therefore.Engine.Compiler;
    using Therefore.Engine.Compiler.Constraints;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;
    using Therefore.Web.Models;

    public class GameController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Play()
        {
            var model = new GameBoard
            {
                Premises = new string[4],
                Results = new Dictionary<string, bool?>
                {
                    { "A", null },
                    { "B", null },
                    { "C", null },
                    { "D", null }
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Play(GameBoard board)
        {
            Expression accumulator = null;
            var nameTable = new List<string> { "A", "B", "C", "D" };
            var debugInfo = new Dictionary<string, object>();
            ViewBag.DebugInfo = debugInfo;

            for (int i = 0; i < board.Premises.Count; i++)
            {
                var key = "Premises[" + i + "]";
                var premiseText = board.Premises[i];

                if (string.IsNullOrWhiteSpace(premiseText))
                {
                    continue;
                }

                try
                {
                    var parser = new Parser();
                    var premiseTree = parser.Parse(premiseText);
                    debugInfo[key + " Parse Tree"] = ToJson(premiseTree);

                    var comparer = StringComparer.OrdinalIgnoreCase;
                    var compilerOptions = new CompilerOptions
                    {
                        Constraints = new Constraint[] {
                            new ParenthesizedNotConstraint(),
                            new SpecificVariablesConstraint(new[] { "A", "B", "C", "D" }, comparer),
                        },
                        VariableNameComparer = comparer,
                    };

                    var compiler = new Compiler(compilerOptions);
                    var premiseExpr = compiler.Compile(premiseTree, nameTable);
                    debugInfo[key + " Expression Tree"] = premiseExpr;

                    accumulator = accumulator == null
                        ? premiseExpr
                        : new AndExpression(premiseExpr, accumulator);
                }
                catch (ParseException ex)
                {
                    ModelState.AddModelError(key, ex.Message);
                    continue;
                }
                catch (CompileException ex)
                {
                    ModelState.AddModelError(key, ex.Message);
                    continue;
                }
            }

            if (ModelState.IsValid)
            {
                var namedResults = new Dictionary<string, bool?>();

                if (accumulator != null)
                {
                    var results = Solver.Solve(accumulator, nameTable.Count);

                    if (results == null)
                    {
                        namedResults = null;
                    }
                    else
                    {
                        for (int var = 0; var < nameTable.Count; var++)
                        {
                            namedResults[nameTable[var]] = results[var];
                        }
                    }
                }

                board.Results = namedResults;
            }

            return View(board);
        }

        [HttpGet]
        public ActionResult Parse(string statement)
        {
            object data;
            try
            {
                var parser = new Parser();
                data = parser.Parse(statement);
            }
            catch (ParseException ex)
            {
                data = new { Error = ex.Message, ex.Offset };
            }

            return Content(ToJson(data), "application/json");
        }

        private string ToJson(object data)
        {
            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { new TokenConverter() });
            return ser.Serialize(data);
        }

        private class TokenConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                var token = (Token)obj;
                var result = new Dictionary<string, object>();

                result["TokenType"] = token.TokenType.ToString();
                result["Span"] = token.Span;
                result["Value"] = token.Value;

                return result;
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get { return new[] { typeof(Token) }; }
            }
        }
    }
}
