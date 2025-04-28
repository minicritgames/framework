using UnityEngine;

namespace Minikit
{
    /// <summary> Helper class with additional functions that aren't included in Unity's Gizmos class. </summary>
    public static class MKGizmos
    {
        public static void DrawArrow(Vector3 _position, Vector3 _direction, float _arrowHeadLength = 0.25f, float _arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(_position, _direction);

            Vector3 right = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 + _arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 - _arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(_position + _direction, right * _arrowHeadLength);
            Gizmos.DrawRay(_position + _direction, left * _arrowHeadLength);
        }
    }
} // Minikit namespace
