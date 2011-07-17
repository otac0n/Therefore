namespace Therefore.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Therefore.Web.Models;
    using Therefore.Engine.Expressions;
    using Therefore.Engine.Parser;
    using Therefore.Engine;
    using System.Web.Script.Serialization;
    using Therefore.Engine.Game;
    using Therefore.Engine.Compiler;

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
                    var premiseTree = Parser.Parse(premiseText);
                    debugInfo[key + " Parse Tree"] = ToJson(premiseTree);

                    var constraintViolations = new ConstraintVisior(new ParenthesizedNotConstraint()).Visit(premiseTree).ToList();
                    foreach (var violation in constraintViolations)
                    {
                        ModelState.AddModelError(key, violation.Message);
                    }

                    var premiseExpr = Compiler.Compile(premiseTree, nameTable, StringComparer.OrdinalIgnoreCase);
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
                data = Parser.Parse(statement);
            }
            catch (ParseException ex)
            {
                data = new { Error = ex.Message, ex.Offset };
            }

            var result = Json(data);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        private string ToJson(ParseTree tree)
        {
            var ser = new JavaScriptSerializer();
            return ser.Serialize(tree);
        }
    }
}
