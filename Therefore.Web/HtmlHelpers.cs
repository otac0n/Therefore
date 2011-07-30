namespace Therefore.Web
{
    using System;
    using System.Web.Mvc;

    public static class HtmlHelpers
    {
        private readonly static Random random = new Random();

        public static int Random(this HtmlHelper html, int maximum)
        {
            lock (random)
            {
                return random.Next(maximum);
            }
        }
    }
}