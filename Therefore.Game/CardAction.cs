namespace Therefore.Game
{
    using System;

    public abstract class CardAction
    {
        private readonly Card card;

        public CardAction(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }

            this.card = card;
        }

        public Card Card
        {
            get { return this.card; }
        }

        public abstract GameState ApplyTo(GameState currentState, string playerId);
    }
}
