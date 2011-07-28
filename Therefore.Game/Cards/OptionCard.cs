namespace Therefore.Game.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OptionCard : Card
    {
        private readonly IList<string> symbols;

        public OptionCard(params string[] symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

            this.symbols = symbols.ToList().AsReadOnly();
        }

        public IList<string> Symbols
        {
            get { return this.symbols; }
        }
    }
}
