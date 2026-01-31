using UnityEngine;

namespace Minikit.Framework
{
    public static class MKMathExtensions
    {
        public static float CubicEaseIn(float _t)
        {
            return _t * _t * _t;
        }

        public static float CubicEaseOut(float _t)
        {
            _t = 1f - _t;
            return 1f - _t * _t * _t;
        }
    }
} // Minikit.Framework namespace
