using System.Collections;
namespace Minesweeper
{
    public class BitArray2D : IEnumerable<bool>
    {
        private BitArray Array;
        public uint Length1 { get; private set; }
        public uint Length2 { get; private set; }
        public BitArray2D(uint length1, uint length2)
        {
            Array = new BitArray((int)(length1 * length2));
            Length1 = length1;
            Length2 = length2;
        }
        public BitArray2D(bool[,] arr)
        {
            Length1 = (uint)arr.GetLength(0);
            Length2 = (uint)arr.GetLength(1);
            Array = new BitArray((int)(Length1 * Length2));
            for (uint x = 0; x < Length1; x++)
            {
                for (uint y = 0; y < Length2; y++)
                {
                    Set(x, y, arr[x, y]);
                }
            }
        }
        public void Set(int x, int y, bool b)
        {
            CheckBounds(x, y);
            Array[x + (int)Length1 * y] = b;
        }
        public void Set(uint x, uint y, bool b)
        {
            CheckBounds(x, y);
            Array[(int)(x + Length1 * y)] = b;
        }
        public bool Get(int x, int y)
        {
            CheckBounds(x, y);
            return Array[x + (int)Length1 * y];
        }
        public bool Get(uint x, uint y)
        {
            CheckBounds(x, y);
            return Array[(int)(x + Length1 * y)];
        }
        public bool this[int x, int y] { get { return Get(x, y); } set { Set(x, y, value); } }
        public bool this[uint x, uint y] { get { return Get(x, y); } set { Set(x, y, value); } }
        public IEnumerator<bool> GetEnumerator() => Array.GetEnumerator().ToGeneric<bool>();
        IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();
        private void CheckBounds(int x, int y)
        {
            if (0 <= x && x < Length1 && 0 <= y && y < Length2)
            {
                return;
            }
            throw new IndexOutOfRangeException();
        }
        private void CheckBounds(uint x, uint y)
        {
            if (x < Length1 && y < Length2)
            {
                return;
            }
            throw new IndexOutOfRangeException();
        }
    }
}