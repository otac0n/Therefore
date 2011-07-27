namespace Therefore.Game
{
    using System.Collections.Generic;
    using System.Linq;

    public class TotalCardsScoringSystem : ScoringSystem
    {
        public override Dictionary<string, int> Score(GameState gameState)
        {
            var scoringPlayers = gameState.FindExistingPlayers();
            var points = gameState.Proof.Sum(p => p.Count);

            return scoringPlayers.ToDictionary(s => s, s => points);
        }
    }
}
