namespace Therefore.Game
{
    public class PlaceOptionCardAction : PlaceCardAction
    {
        public PlaceOptionCardAction(OptionCard card, int symbol, int premise, int index)
            : base(new PlacementCard(card.Symbols[symbol]), premise, index)
        {
        }
    }
}
