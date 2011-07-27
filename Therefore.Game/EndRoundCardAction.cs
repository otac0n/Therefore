namespace Therefore.Game
{
    public class EndRoundCardAction : CardAction
    {
        public EndRoundCardAction(ErgoCard card)
            : base(card)
        {
        }

        public override GameState ApplyTo(GameState currentState, string playerId)
        {
            return currentState.EndRound().Discard(playerId, this.Card);
        }
    }
}
