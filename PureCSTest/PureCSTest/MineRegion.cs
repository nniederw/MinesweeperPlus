using System.Diagnostics.CodeAnalysis;

namespace Minesweeper
{
    struct MineRegion
    {
        public int Mines;
        public (int x, int y)[] Positions;
        public MineRegion()
        {
            Mines = 0;
            Positions = new (int x, int y)[0];
        }
        public MineRegion(int mines, (int x, int y)[] positions)
        {
            Mines = mines;
            Positions = positions; //todo make copy
        }
        public override int GetHashCode()
        {
            int result = 0;
            for (int i = 0; i < Positions.Length; i++)
            {
                result <<= 1;
                result += Positions[i].x + Positions[i].y;
            }
            return (result << 1) + Mines;
        }
        public bool Equals(MineRegion other)
            => (Mines == other.Mines) && Positions.Intersect(other.Positions).Count() == Positions.Length;
        public static bool operator ==(MineRegion a, MineRegion b)
        => a.Equals(b);
        public static bool operator !=(MineRegion a, MineRegion b)
        => !(a == b);
        public bool Contains(MineRegion other) //remove if not needed
        {
            if (this == other) return true;
            return Positions.Intersect(other.Positions).Count() == other.Positions.Length;
        }
        public override bool Equals([NotNullWhen(true)] object? obj) => throw new Exception("Don't use this shitty thing");
    }
}