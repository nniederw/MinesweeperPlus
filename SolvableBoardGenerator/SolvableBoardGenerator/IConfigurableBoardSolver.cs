namespace Minesweeper
{
    public interface IConfigurableBoardSolver
    {
        public void DoMineCount(bool doit);
        public void DoNon1SizedPatterns(bool doit);
    }
}