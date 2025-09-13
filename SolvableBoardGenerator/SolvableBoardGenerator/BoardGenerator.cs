namespace Minesweeper
{
    public static class BoardGenerator
    {
        public static Board GetFirstBeginnerBoard() => GetFirstBoardOfType(BoardType.Beginner);
        public static Board GetFirstBoardOfType(BoardType boardType) => new Board(GetAllBoardsOfSize(boardType).First());
        public static IEnumerable<bool[,]> GetAllBoardsOfSize(BoardType boardType) => GetAllBoardsOfSize(boardType.Width, boardType.Height, boardType.Mines);
        public static IEnumerable<bool[,]> GetAllBoardsOfSize(uint sizeX, uint sizeY, uint mines)
        {
            var permuts = Combinatorics.GetCombinationsIterative(mines, sizeX * sizeY).Select(i => BoardFromSingleDArr(i, sizeX, sizeY));
            foreach (var perm in permuts.Where(IsEarliestBoard))
            {
                yield return perm;
            }
        }
        /* Info for solvable Expert boards: 
         * [seed, solvable from (x,y)]
         * 4, (0,0)
         * 17, (0,10)
         * 20, (3,2)
         * 29, (11,0)
         * 37, (0,2)
         * 44, (0,0)
         * 45, (4,5)
         * 53, (0,9)
         * 54, (2,8)
         * 55, (3,9)
         * 58, (0,12)
         * 67, (3,11)
         * 72, (6,4)
         * 75, (0,14)
         * 77, (2,15)
         */
        /* Info for solvable BoardType(100, 100, 2100) boards: 
         * [seed, solvable from (x,y)]
         *  121, (0,0)
         *  3828, (0,3)
         *  7865, (0,0)
         *  9415, (0,0)
         *  14399, (0,2)
         *  15548, (0,0)
         *  16629, (0,3)
         *  30775, (0,4)
         *  31799, (0,0)
         *  35750, (0,0)
         *  38850, (0,5)
         *  42540, (0,3)
         */
        public static Board GetRandomSeededBoard(BoardType boardType, int? seed = null) => new Board(GetRandomSeededField(boardType, seed));
        public static bool[,] GetRandomSeededField(BoardType boardType, int? seed = null) => GetRandomSeededBoard(boardType.Width, boardType.Height, boardType.Mines, seed);
        public static bool[,] GetRandomSeededBoard(uint sizeX, uint sizeY, uint mines, int? seed = null)
        {
            Random rng = seed != null ? new Random(seed.Value) : new Random();
            var minesUsed = 0;
            var result = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    var leftSquares = (sizeX - x) * sizeY - y;
                    var leftMines = mines - minesUsed;
                    if (leftMines == 0)
                    {
                        result[x, y] = false;
                        continue;
                    }
                    if (leftMines == leftSquares)
                    {
                        result[x, y] = true;
                        continue;
                    }
                    var mineDensity = ((float)leftMines) / leftSquares;
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
                uint y = i / sizeX;
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
        public static IEnumerable<bool[,]> GetAllBoardsUpToSize(uint sizeA, uint sizeB)
        {
            if (sizeA < sizeB)
            {
                uint t = sizeA;
                sizeA = sizeB;
                sizeB = t;
            }
            foreach (var sizes in Combinatorics.AllDouble32bNaturalNumbersGreaterThan(0).Skip(1).Where(i => i.a >= i.b))
            {
                if (sizes.a > sizeA && sizes.b > sizeB)
                {
                    yield break;
                }
                if (sizes.a > sizeA || sizes.b > sizeB)
                {
                    continue;
                }
                foreach (var perm in GetAllBoardsOfSize(sizes.a, sizes.b))
                {
                    yield return perm;
                }
            }
        }
        public static bool IsEarliestBoard(bool[,] mines)
        {
            var mirX = SymetricBoardTools.MirrorOnXAxis(mines);
            var mirY = SymetricBoardTools.MirrorOnYAxis(mines);
            var mirXY = SymetricBoardTools.MirrorOnBothAxis(mines);
            var pool = new List<bool[,]>() { mirX, mirY, mirXY };
            if (mines.GetLength(0) == mines.GetLength(1))
            {
                var rot90 = SymetricBoardTools.RotateSquareBoard90Degrees(mines);
                var rot270 = SymetricBoardTools.RotateSquareBoard270Degrees(mines);
                var mirD = SymetricBoardTools.MirrorMainDiagonal(mines);
                var mirAD = SymetricBoardTools.MirrorAntiDiagonal(mines);
                pool.Add(rot90);
                pool.Add(rot270);
                pool.Add(mirD);
                pool.Add(mirAD);
            }
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