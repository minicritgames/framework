using UnityEngine;

namespace Minikit
{
    /// <summary> Helper class with additional functions that aren't included in Unity's Gizmos class. </summary>
    public static class MKGizmos
    {
        public static void DrawArrow(Vector3 _position, Vector3 _direction, float _arrowHeadLength = 0.25f, float _arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(_position, _direction);

            if (_direction.sqrMagnitude < 1e-6f)
            {
                return;
            }

            Vector3 right = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 + _arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 - _arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(_position + _direction, right * _arrowHeadLength);
            Gizmos.DrawRay(_position + _direction, left * _arrowHeadLength);
        }

        public static void DrawCross(Vector3 _position, float _size)
        {
            float half = _size * 0.5f;
            Gizmos.DrawLine(_position - (Vector3.right * half), _position + (Vector3.right * half));
            Gizmos.DrawLine(_position - (Vector3.up * half), _position + (Vector3.up * half));
            Gizmos.DrawLine(_position - (Vector3.forward * half), _position + (Vector3.forward * half));
        }

        public static Color WithAlpha(Color _color, float _alpha)
        {
            _color.a = _alpha;
            return _color;
        }
    }
} // Minikit namespace
