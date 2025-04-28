using System;

namespace Minikit
{
    public struct int2 : IEquatable<int2>
    {
        public static int2 none = new(int.MinValue, int.MinValue);
        public static int2 left = new(-1, 0);
        public static int2 right = new(1, 0);
        public static int2 up = new(0, 1);
        public static int2 down = new(0, -1);

        public int x;
        public int y;


        public int2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator==(int2 _a, int2 _b)
        {
            return _a.Equals(_b);
        }

        public static bool operator!=(int2 _a, int2 _b)
        {
            return !(_a == _b);
        }

        public static int2 operator+(int2 _a, int2 _b)
        {
            return new int2(_a.x + _b.x, _a.y + _b.y);
        }

        public static int2 operator-(int2 _a, int2 _b)
        {
            return new int2(_a.x - _b.x, _a.y - _b.y);
        }

        public override bool Equals(object _obj)
        {
            return base.Equals(_obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public bool Equals(int2 _other)
        {
            return x == _other.x && y == _other.y;
        }
    }
} // Minikit namespace
