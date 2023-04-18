namespace Minesweeper
{
    public class Board
    {
        private int SizeX = 10;
        private int SizeY = 10;
        private int Mines = 30;
        private int MinesLeft = 30;
        private sbyte[,] MineField; //0-8 numbers, 9 meaning mine
        public static sbyte[,] GenerateField(int seed, int sizeX, int sizeY, int mines)
        {
            sbyte[,] mineField = new sbyte[sizeX, sizeY];
            System.Random rand = new System.Random(seed);
            int leftSquares = sizeX * sizeY;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (mines != leftSquares)
                    {
                        mineField[x, y] = (sbyte)(rand.NextDouble() <= ((double)mines) / leftSquares ? 9 : 0);
                        if (mineField[x, y] == 9) { mines--; }
                        leftSquares--;
                    }
                    else
                    {
                        mineField[x, y] = 9;
                        leftSquares--;
                    }
                }
            }
            CalcNumbers(mineField);
            return mineField;
        }
        private static void CalcNumbers(sbyte[,] mineField)
        {
            int sizeX = mineField.GetLength(0);
            int sizeY = mineField.GetLength(1);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (mineField[x, y] != 9)
                    {
                        mineField[x, y] = (sbyte)AdjMines(mineField, x, y);
                    }
                }
            }
        }
        private static int AdjMines(sbyte[,] mineField, int x, int y)
        {
            int mines = 0;
            try { mines += mineField[x - 1, y + 1] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x + 0, y + 1] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x + 1, y + 1] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x + 1, y + 0] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x + 1, y - 1] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x + 0, y - 1] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x - 1, y - 1] == 9 ? 1 : 0; } catch { }
            try { mines += mineField[x - 1, y + 0] == 9 ? 1 : 0; } catch { }
            return mines;
        }
    }
}