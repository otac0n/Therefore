namespace Therefore.Game
{
    using System;

    public class PlaceCardAction : CardAction
    {
        private readonly int premise;
        private readonly int index;
        private readonly PlacementCard card;

        public PlaceCardAction(PlacementCard card, int premise, int index)
            : base(card)
        {
            this.card = card;

            if (premise < 0)
            {
                throw new ArgumentOutOfRangeException("premise");
            }

            this.premise = premise;

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.index = index;
        }

        public new PlacementCard Card
        {
            get
            {
                return this.card;
            }
        }

        public override GameState ApplyTo(GameState currentState, string playerId)
        {
            return currentState.PlaceCardAt(this.card, this.premise, this.index).Discard(playerId, this.Card);
        }
    }
}
