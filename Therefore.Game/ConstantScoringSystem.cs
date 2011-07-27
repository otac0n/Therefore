namespace Therefore.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ConstantScoringSystem : ScoringSystem
    {
        private readonly int points;

        public ConstantScoringSystem(int points = 1)
        {
            if (points <= 0)
            {
                throw new ArgumentOutOfRangeException("points");
            }

            this.points = points;
        }

        public override Dictionary<string, int> Score(GameState gameState)
        {
            var scoringPlayers = gameState.FindExistingPlayers();

            return scoringPlayers.ToDictionary(s => s, s => this.points);
        }
    }
}
