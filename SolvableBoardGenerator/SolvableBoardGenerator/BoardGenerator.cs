namespace Minesweeper
{
    public static class BoardGenerator
    {
        public static IEnumerable<bool[,]> GetAllBoardsOfSize(uint sizeX, uint sizeY, uint mines)
        {
            var permuts = Combinatorics.GetCombinationsIterative(mines, sizeX * sizeY).Select(i => BoardFromSingleDArr(i, sizeX, sizeY));
            foreach (var perm in permuts.Where(IsEarliestBoard))
            {
                yield return perm;
            }
        }
        public static bool[,] GetRandomSeededBoard(uint sizeX, uint sizeY, uint mines, int? seed = null)
        {
            Random rng = seed != null ? new Random(seed.Value) : new Random();
            var minesUsed = 0;
            var result = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {

                    var mineDensity = ((float)mines - minesUsed) / ((sizeX - x) * sizeY - y);
                    result[x, y] = rng.NextSingle() <= mineDensity;
                    if (result[x, y])
                    {
                        minesUsed++;
                    }
                }
            }
            return result;
        }
        public static bool[,] BoardFromSingleDArr(bool[] mines, uint sizeX, uint sizeY)
        {
            if (mines.Length != sizeX * sizeY)
            {
                throw new Exception($"array length missmatch in {nameof(BoardFromSingleDArr)}");
            }
            var result = new bool[sizeX, sizeY];
            for (uint i = 0; i < mines.Length; i++)
            {
                uint x = i % sizeX;
                uint y = i / sizeY;
                result[x, y] = mines[i];
            }
            return result;
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
            var rot90 = SymetricBoardTools.RotateSquareBoard90Degrees(mines);
            var rot270 = SymetricBoardTools.RotateSquareBoard270Degrees(mines);
            var mirD = SymetricBoardTools.MirrorMainDiagonal(mines);
            var mirAD = SymetricBoardTools.MirrorAntiDiagonal(mines);
            var pool = new List<bool[,]>() { mirX, mirY, mirXY, rot90, rot270, mirD, mirAD };
            for (int x = 0; x < mines.GetLength(0); x++)
            {
                for (int y = 0; y < mines.GetLength(1); y++)
                {
                    var c = pool.Count;
                    for (int i = c - 1; i >= 0; i--)
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