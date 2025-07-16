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
                result[x, y] = mines[(sizeX - x - 1), y]
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
                result[x, y] = mines[x, (sizeY - y - 1)]
            }
        }
        return result;
    }
    public static bool[,] MirrorOnBothAxis(bool[,] mines) => MirrorOnYAxis(MirrorOnXAxis(mines));
    /// <summary>
    /// Counter Clockwise
    /// </summary>
    public static bool[,] RotateSquareBoard90Degrees(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        if (sizeX != sizeY) { throw new Exception($"{nameof(RotateSquareBoard90Degrees)} can only be called for square boards."); }
        throw new NotImplementedException();
    }
    /// <summary>
    /// Counter Clockwise
    /// </summary>
    public static bool[,] RotateSquareBoard270Degrees(bool[,] mines)
    {
        var sizeX = mines.GetLength(0);
        var sizeY = mines.GetLength(1);
        if (sizeX != sizeY) { throw new Exception($"{nameof(RotateSquareBoard90Degrees)} can only be called for square boards."); }
        throw new NotImplementedException();
    }
}