namespace Minesweeper
{
    public class SimpleBoardSolver : BaseBoardSolver
    {
        public SimpleBoardSolver() { }
        public SimpleBoardSolver(Board board, bool verboseLogging = false) : base(board, verboseLogging) { }
        public override IBoardSolver Construct(Board board, bool verboseLogging = false) => new SimpleBoardSolver(board, verboseLogging);
        public override SolvabilityClass GetSolvabilityClass => SolvabilityClass.Partial;
    }
}