namespace Therefore.Game
{
    using System;

    public class RemoveCardAction : CardAction
    {
        private readonly int premise;
        private readonly int index;
        private readonly ExsculpoCard card;

        public RemoveCardAction(ExsculpoCard card, int premise, int index)
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

        public new ExsculpoCard Card
        {
            get
            {
                return this.card;
            }
        }

        public override GameState ApplyTo(GameState currentState, string playerId)
        {
            return currentState.RemoveCardAt(this.premise, this.index).Discard(playerId, this.Card);
        }
    }
}
