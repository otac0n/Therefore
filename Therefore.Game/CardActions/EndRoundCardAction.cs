namespace Therefore.Game.CardActions
{
    using Therefore.Game.Cards;

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
