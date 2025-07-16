namespace Minesweeper
{
    public static class BoardGenerator
    {
        public static IEnumerable<bool[,]> GetAllBoardsOfSize(uint sizeX, uint sizeY, uint mines)
        {
            //Combinatorics.
            throw new NotImplementedException();
        }
        public static IEnumerable<bool[,]> GetAllBoardsOfSize(uint sizeX, uint sizeY)
        {
            for (uint mines = 1; mines < sizeX * sizeY; mines++)
            {
                foreach (var board in GetAllBoardsOfSize(sizeX, sizeY, mines))
                {
                    yield return board;
                }
            }
        }
        public static bool IsEarliestBoard(bool[,] mines)
        {
            var mirX = SymetricBoardTools.MirrorOnXAxis(mines);
            var mirY = SymetricBoardTools.MirrorOnYAxis(mines);
            var mirXY = SymetricBoardTools.MirrorOnBothAxis(mines);
            var pool = new List<bool[,]>() { mirX, mirY, mirXY };
            //todo add rotation
            for (int x = 0; x < mines.GetLength(0); x++)
            {
                for (int y = 0; y < mines.GetLength(1); y++)
                {
                    var c = pool.Count;
                    for (int i = c; i >= 0; i--)
                    {
                        if (!mines[x, y] && pool[i][x, y]) return false;
                        if (mines[x, y] && !pool[i][x, y])
                        {
                            pool.RemoveAt(i);
                        }
                    }
                    if (!pool.Any()) return true;
                }
            }
            return true;
        }
    }
}