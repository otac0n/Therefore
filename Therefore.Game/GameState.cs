namespace Therefore.Game
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Therefore.Engine.Compiler;
    using Therefore.Engine.Parser;
    using Premise = System.Collections.ObjectModel.ReadOnlyCollection<PlacementCard>;

    public sealed class GameState
    {
        private static Parser parser;
        private static Compiler compiler;

        private readonly ReadOnlyCollection<string> playerIds;
        private readonly ReadOnlyCollection<Card> deck;
        private readonly ReadOnlyDictionary<string, ReadOnlyCollection<Card>> hands;
        private readonly int dealer;
        private readonly int turn;
        private readonly Premise[] proof;
        private readonly bool isRoundOver;

        public GameState(IList<string> playerIds)
        {
            this.playerIds = playerIds.ToList().AsReadOnly();

            var deck = ShuffleNewDeck();
            var hands = DealCards(deck, this.playerIds);
            var turn = GetNextPlayer(this.dealer, this.playerIds.Count);

            this.deck = deck.ToList().AsReadOnly();
            this.hands = hands.ToDictionary(h => h.Key, h => h.Value.AsReadOnly()).AsReadOnly();
            this.dealer = turn;
            this.turn = turn;
            this.proof = (from i in Enumerable.Range(0, 4)
                          select new Premise(new List<PlacementCard>())).ToArray();
            this.isRoundOver = false;
        }

        private GameState(
            ReadOnlyCollection<string> playerIds,
            ReadOnlyCollection<Card> deck,
            ReadOnlyDictionary<string, ReadOnlyCollection<Card>> hands,
            int dealer,
            int turn,
            Premise[] proof,
            bool isRoundOver)
        {
            this.playerIds = playerIds;
            this.deck = deck;
            this.hands = hands;
            this.dealer = dealer;
            this.turn = turn;
            this.proof = proof;
            this.isRoundOver = isRoundOver;
        }

        public string CurrentPlayerId
        {
            get
            {
                return this.playerIds[this.turn];
            }
        }

        public IDictionary<string, ReadOnlyCollection<Card>> Hands
        {
            get
            {
                return this.hands;
            }
        }

        public bool IsValid
        {
            get
            {
                var parser = GetParser();
                var compiler = GetCompiler();

                foreach (var premise in this.proof)
                {
                    if (premise.Count == 0)
                    {
                        continue;
                    }

                    try
                    {
                        var parseTree = parser.Parse(string.Join(string.Empty, from c in premise select c.Symbol));
                        compiler.Compile(parseTree, new List<string>());
                    }
                    catch (ParseException)
                    {
                        return false;
                    }
                    catch (CompileException)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsRoundOver
        {
            get
            {
                return this.isRoundOver;
            }
        }

        private static Compiler GetCompiler()
        {
            if (compiler == null)
            {
                compiler = new Compiler();
            }

            return compiler;

        }

        private static Parser GetParser()
        {
            if (parser == null)
            {
                parser = new Parser();
            }

            return parser;
        }

        public GameState StartNewRound()
        {
            var deck = ShuffleNewDeck();
            var hands = DealCards(deck, this.playerIds);
            var turn = GetNextPlayer(this.dealer, this.playerIds.Count);

            return new GameState(
                this.playerIds,
                deck.ToList().AsReadOnly(),
                hands.ToDictionary(h => h.Key, h => h.Value.AsReadOnly()).AsReadOnly(),
                turn,
                turn,
                new Premise[this.proof.Length],
                false);
        }

        private static int GetNextPlayer(int currentPlayer, int playerCount)
        {
            return (currentPlayer + 1) % playerCount;
        }

        private static Dictionary<string, List<Card>> DealCards(Stack<Card> deck, IList<string> playerIds)
        {
            var hands = playerIds.ToDictionary(p => p, p => new List<Card>());

            foreach (var playerId in playerIds)
            {
                for (int i = 0; i < 7; i++)
                {
                    var card = deck.Pop();
                    hands[playerId].Add(card);
                }
            }

            return hands;
        }

        private static Stack<Card> ShuffleNewDeck()
        {
            var cards = new List<Card>();
            for (var i = 0; i < 4; i++)
            {
                cards.Add(new PlacementCard("A"));
                cards.Add(new PlacementCard("B"));
                cards.Add(new PlacementCard("C"));
                cards.Add(new PlacementCard("D"));

                cards.Add(new PlacementCard("·"));
                cards.Add(new PlacementCard("∨"));
                cards.Add(new PlacementCard("⇒"));
                cards.Add(new PlacementCard("~"));
                cards.Add(new PlacementCard("~"));

                cards.Add(new ExsculpoCard());
            }

            for (int i = 0; i < 6; i++)
            {
                cards.Add(new OptionCard("(", ")"));
            }

            cards.Add(new OptionCard("A", "B", "C", "D"));
            cards.Add(new OptionCard("·", "∨", "⇒", "~"));

            var deck = new Stack<Card>();
            var rand = new Random();
            var count = cards.Count;
            for (var i = 0; i < count; i++)
            {
                var card = rand.Next(cards.Count);
                deck.Push(cards[card]);
                cards.RemoveAt(card);
            }

            return deck;
        }

        public GameState DrawCards(string playerId)
        {
            var deck = new Stack<Card>(this.deck);
            var hands = this.hands.ToDictionary(h => h.Key, h => h.Value.ToList());

            var hand = hands[playerId];
            while (hand.Count < 7 && deck.Count > 0)
            {
                hand.Add(deck.Pop());
            }

            var isRoundOver = this.isRoundOver;
            if (hand.Count < 7)
            {
                isRoundOver = true;
            }

            return new GameState(
                this.playerIds,
                deck.ToList().AsReadOnly(),
                hands.ToDictionary(h => h.Key, h => h.Value.AsReadOnly()).AsReadOnly(),
                this.dealer,
                this.turn,
                this.proof,
                isRoundOver);
        }

        public GameState AdvanceTurn()
        {
            var turn = GetNextPlayer(this.turn, this.playerIds.Count);

            return new GameState(
                this.playerIds,
                this.deck,
                this.hands,
                this.dealer,
                turn,
                this.proof,
                this.isRoundOver);
        }

        public GameState PlaceCardAt(PlacementCard card, int premise, int index)
        {
            var proof = (from p in this.proof
                         select p.ToList()).ToArray();

            proof[premise].Insert(index, card);

            return new GameState(
                this.playerIds,
                this.deck,
                this.hands,
                this.dealer,
                this.turn,
                proof.Select(p => p.AsReadOnly()).ToArray(),
                this.isRoundOver);
        }

        public GameState Discard(string playerId, Card card)
        {
            var hands = this.hands.ToDictionary(h => h.Key, h => h.Value.ToList());
            var hand = hands[playerId];
            hand.Remove(card);

            return new GameState(
                this.playerIds,
                this.deck,
                hands.ToDictionary(h => h.Key, h => h.Value.AsReadOnly()).AsReadOnly(),
                this.dealer,
                this.turn,
                this.proof,
                this.isRoundOver);
        }

        public GameState RemoveCardAt(int premise, int index)
        {
            var proof = (from p in this.proof
                         select p.ToList()).ToArray();

            proof[premise].RemoveAt(index);

            return new GameState(
                this.playerIds,
                this.deck,
                this.hands,
                this.dealer,
                this.turn,
                proof.Select(p => p.AsReadOnly()).ToArray(),
                this.isRoundOver);
        }
    }
}
