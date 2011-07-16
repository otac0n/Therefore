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

            for (int i = 0; i < board.Premises.Count; i++)
            {
                var premiseText = board.Premises[i];

                if (string.IsNullOrWhiteSpace(premiseText))
                {
                    continue;
                }

                try
                {
                    var premiseTree = Parser.Parse(premiseText);
                    var premiseExpr = Compiler.Compile(premiseTree, nameTable, StringComparer.OrdinalIgnoreCase);
                    accumulator = accumulator == null
                        ? premiseExpr
                        : new AndExpression(premiseExpr, accumulator);
                }
                catch (ParseException ex)
                {
                    ModelState.AddModelError("Premises[" + i + "]", ex.Message);
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
    }
}
