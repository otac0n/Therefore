namespace Therefore.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class GameBoard
    {
        public IList<string> Premises { get; set; }

        public Dictionary<string, bool?> Results { get; set; }
    }
}
