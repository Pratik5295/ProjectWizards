using System;

namespace Team.Gameplay.GridSystem
{
    [System.Serializable]
    public struct TileID : IEquatable<TileID>
    {
        public int x;
        public int y;

        //Constructor 
        public TileID(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(TileID other)
        {
           return this.x == other.x && this.y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is TileID  && Equals((TileID)obj);
        }

        public override int GetHashCode()
        {
            unchecked // Allow overflow
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                return hash;
            }
        }

        public static bool operator ==(TileID a, TileID b) => a.Equals(b);
        public static bool operator !=(TileID a, TileID b) => !a.Equals(b);

        public override string ToString() => $"({x}, {y})";
    }
}
