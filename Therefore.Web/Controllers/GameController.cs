namespace Therefore.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Therefore.Engine.Parser;
    using Therefore.Game;

    public class GameController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            var gameId = MvcApplication.GetNextGameId();
            var cacheKey = "Game" + gameId;
            var game = new Game(new[] { "A", "B", "C", "D" });
            HttpContext.Cache[cacheKey] = game;

            return RedirectToAction("Play", new { id = gameId });
        }

        public ActionResult Play(int id)
        {
            var cacheKey = "Game" + id;
            var game = (Game)HttpContext.Cache[cacheKey];
            return View(game);
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
