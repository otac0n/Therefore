namespace Therefore.Game
{
    using System.Collections.Generic;

    public abstract class ScoringSystem
    {
        public abstract Dictionary<string, int> Score(GameState gameState);
    }
}
