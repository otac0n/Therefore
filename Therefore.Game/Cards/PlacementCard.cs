namespace Therefore.Game.Cards
{
    using System.Diagnostics;

    [DebuggerDisplay("Card: {Symbol}")]
    public class PlacementCard : Card
    {
        private string symbol;

        public PlacementCard(string symbol)
        {
            this.symbol = symbol;
        }

        public string Symbol
        {
            get { return this.symbol; }
        }
    }
}
