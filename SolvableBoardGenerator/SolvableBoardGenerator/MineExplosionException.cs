namespace Minesweeper
{
    public class MineExplosionException : Exception
    {
        public MineExplosionException() : base() { }
        public MineExplosionException(string s) : base(s) { }
    }
}