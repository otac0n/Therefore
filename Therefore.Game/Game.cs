namespace Therefore.Game
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Game
    {
        private GameState gameState;

        public Game(IList<string> playerIds)
        {
            if (playerIds == null)
            {
                throw new ArgumentNullException("playerIds");
            }

            this.gameState = new GameState(this, playerIds);
        }

        public string CurrentPlayer
        {
            get { return this.gameState.CurrentPlayerId; }
        }

        public IDictionary<string, ReadOnlyCollection<Card>> Hands
        {
            get
            {
                return this.gameState.Hands;
            }
        }

        public void StartNewRound()
        {
            this.gameState = this.gameState.StartNewRound();
        }

        public void PlayCards(string playerId, IList<CardAction> actions)
        {
            if (playerId != this.gameState.CurrentPlayerId)
            {
                throw new InvalidOperationException("It is not your turn.");
            }

            if (actions.Count > 2)
            {
                throw new InvalidOperationException("Cannot play more than 2 cards per turn.");
            }

            var distinctCards = new HashSet<Card>();
            foreach (var action in actions)
            {
                if (!this.gameState.Hands[playerId].Contains(action.Card))
                {
                    throw new InvalidOperationException("This is not your card.");
                }

                if (distinctCards.Contains(action.Card))
                {
                    throw new InvalidOperationException("You cannot play the same card twice.");
                }

                distinctCards.Add(action.Card);
            }

            var intermediaryState = this.gameState;
            foreach (var action in actions)
            {
                intermediaryState = action.ApplyTo(intermediaryState, playerId);
            }

            intermediaryState = intermediaryState.DrawCards(playerId).AdvanceTurn();

            if (!intermediaryState.IsValid)
            {
                throw new InvalidOperationException("The play you have attempted puts the game in an invalid state.");
            }

            this.gameState = intermediaryState;
        }
    }
}
