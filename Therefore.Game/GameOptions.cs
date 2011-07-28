namespace Therefore.Game
{
    using Therefore.Engine.Compiler;
    using Therefore.Engine.Parser;
    using Therefore.Game.ScoringSystems;

    public class GameOptions
    {
        public CompilerOptions CompilerOptions
        {
            get;
            set;
        }

        public ParserOptions ParserOptions
        {
            get;
            set;
        }

        public ScoringSystem ScoringSystem
        {
            get;
            set;
        }
    }
}
