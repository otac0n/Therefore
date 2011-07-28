namespace Therefore.Game
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Therefore.Engine.Compiler;
    using Therefore.Engine.Parser;
    using Therefore.Game.CardActions;
    using Therefore.Game.Cards;
    using Therefore.Game.ScoringSystems;

    public class Game
    {
        private readonly Parser parser;
        private readonly Compiler compiler;
        private readonly ScoringSystem scoringSystem;

        private GameState gameState;

        public Game(IList<string> playerIds, GameOptions gameOptions = null)
        {
            if (playerIds == null)
            {
                throw new ArgumentNullException("playerIds");
            }

            if (gameOptions == null || gameOptions.ScoringSystem == null)
            {
                this.scoringSystem = new AverageCardsScoringSystem();
            }
            else
            {
                scoringSystem = gameOptions.ScoringSystem;
            }

            this.parser = new Parser(gameOptions == null ? null : gameOptions.ParserOptions);
            this.compiler = new Compiler(gameOptions == null ? null : gameOptions.CompilerOptions);

            this.gameState = new GameState(this, playerIds);
        }

        public string CurrentPlayer
        {
            get
            {
                return this.gameState.CurrentPlayerId;
            }
        }

        public IDictionary<string, ReadOnlyCollection<Card>> Hands
        {
            get
            {
                return this.gameState.Hands;
            }
        }

        public IList<ReadOnlyCollection<PlacementCard>> Proof
        {
            get
            {
                return this.gameState.Proof;
            }
        }

        public IList<string> PlayerIds
        {
            get
            {
                return this.gameState.PlayerIds;
            }
        }

        public IDictionary<string, int> Scores
        {
            get
            {
                return this.gameState.Scores;
            }
        }


        internal Compiler Compiler
        {
            get
            {
                return this.compiler;
            }
        }

        internal Parser Parser
        {
            get
            {
                return this.parser;
            }
        }

        public ScoringSystem ScoringSystem
        {
            get
            {
                return this.scoringSystem;
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
