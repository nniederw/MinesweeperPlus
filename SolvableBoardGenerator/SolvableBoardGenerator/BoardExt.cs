namespace Minesweeper
{
    public static class BoardExt
    {
        public static BitArrayBoard ToBitArrayBoard(this Board board)
        {
            var numbers = board.CheatGetNumbers();
            int sizeX = numbers.GetLength(0);
            int sizeY = numbers.GetLength(1);
            BitArray2D mines = new BitArray2D((uint)sizeX, (uint)sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    mines[x, y] = numbers[x, y] == Board.MineSByte;
                }
            }
            return new BitArrayBoard(mines, board.GetStartClears().ToList());
        }
    }
}