using System.Drawing;

namespace Minesweeper
{
    public static class BoardConverter
    {
        public static bool[,] BinaryIntsToBools(int[,] mines)
        {
            var sizeX = mines.GetLength(0);
            var sizeY = mines.GetLength(1);
            var result = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    result[x, y] = mines[x, y] == 1 ? true : mines[x, y] == 0 ? false : throw new Exception($"Value {mines[x, y]} is unexpected for the function {nameof(BinaryIntsToBools)}");
                }
            }
            return result;
        }
        public static void PrintBoolArray(bool[,] arr)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                for (int x = 0; x < arr.GetLength(0); x++)
                {
                    Console.Write(arr[x, y] ? 1 : 0);
                }
                Console.WriteLine();
            }
        }
        public static void PrettyPrintBoard(Board board, bool[,] solvability)
        {
            var numbers = board.CheatGetNumbers();
            for (int y = 0; y < solvability.GetLength(1); y++)
            {
                for (int x = 0; x < solvability.GetLength(0); x++)
                {
                    var n = numbers[x, y];
                    ConsoleColor? cc = n == 9 ? null : solvability[x, y] ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
                    Ext.ConsoleWriteColor(n == 9 ? "X" : n.ToString(), cc);
                }
                Console.WriteLine();
            }
        }
    }
}