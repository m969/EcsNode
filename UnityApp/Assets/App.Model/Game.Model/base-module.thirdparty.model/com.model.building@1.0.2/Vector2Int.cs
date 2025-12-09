using System;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 二维整型坐标结构
    /// </summary>
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"({x}, {y})";

        public override int GetHashCode() => HashCode.Combine(x, y);

        public override bool Equals(object? obj)
        {
            if (obj is Vector2Int other) return x == other.x && y == other.y;
            return false;
        }

        public static bool operator ==(Vector2Int a, Vector2Int b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Vector2Int a, Vector2Int b) => !(a == b);
    }
}
