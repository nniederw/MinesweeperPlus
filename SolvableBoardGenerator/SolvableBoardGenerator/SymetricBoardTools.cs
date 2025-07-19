public static class SymetricBoardTools
{
    public static bool[,] MirrorOnXAxis(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        var result = new bool[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                result[x, y] = mines[(sizeX - x - 1), y];
            }
        }
        return result;
    }
    public static bool[,] MirrorOnYAxis(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        var result = new bool[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                result[x, y] = mines[x, (sizeY - y - 1)];
            }
        }
        return result;
    }
    public static bool[,] MirrorOnBothAxis(bool[,] mines) => MirrorOnYAxis(MirrorOnXAxis(mines));

    //chatgpt recoms:
    public static bool[,] MirrorMainDiagonal(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        if (sizeX != sizeY) { throw new Exception($"{nameof(MirrorMainDiagonal)} can only be called for square boards."); }
        int n = sizeX;
        var result = new bool[n, n];
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                result[x, y] = mines[y, x];
            }
        }
        return result;
    }
    public static bool[,] MirrorAntiDiagonal(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        if (sizeX != sizeY) { throw new Exception($"{nameof(MirrorAntiDiagonal)} can only be called for square boards."); }
        int n = sizeX;
        var result = new bool[n, n];
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                result[x, y] = mines[n - 1 - y, n - 1 - x];
            }
        }
        return result;
    }
    /// <summary>
    /// Counter Clockwise
    /// </summary>
    public static bool[,] RotateSquareBoard90Degrees(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        if (sizeX != sizeY) { throw new Exception($"{nameof(RotateSquareBoard90Degrees)} can only be called for square boards."); }
        int n = sizeX;
        var result = new bool[sizeX, sizeY];
        for (int x = 0; x < n; x++) //transpose
        {
            for (int y = 0; y < n; y++)
            {
                result[x, y] = mines[y, x];
            }
        }
        for (int x = 0; x < n; x++) //reverse
        {
            for (int y = 0; y < n / 2; y++)
            {
                bool tmp = result[x, y];
                result[x, y] = result[x, n - y - 1];
                result[x, n - y - 1] = tmp;
            }
        }
        return result;
    }
    /// <summary>
    /// Counter Clockwise
    /// </summary>
    public static bool[,] RotateSquareBoard270Degrees(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        if (sizeX != sizeY) { throw new Exception($"{nameof(RotateSquareBoard270Degrees)} can only be called for square boards."); }
        int n = sizeX;
        var result = new bool[sizeX, sizeY];
        for (int x = 0; x < n; x++) //transpose
        {
            for (int y = 0; y < n; y++)
            {
                result[x, y] = mines[y, x];
            }
        }
        for (int y = 0; y < n; y++) //reverse
        {
            for (int x = 0; x < n / 2; x++)
            {
                bool tmp = result[x, y];
                result[x, y] = result[n - x - 1, y];
                result[n - x - 1, y] = tmp;
            }
        }
        return result;
    }
}