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

        public static void DrawCollider(Collider _collider, bool _solid = false)
        {
            if (!_collider)
            {
                return;
            }

            Transform transform = _collider.transform;
            Vector3 scale = transform.lossyScale;
            scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            Matrix4x4 previousMatrix = Gizmos.matrix;

            switch (_collider)
            {
                case BoxCollider box:
                    Gizmos.matrix = transform.localToWorldMatrix;
                    if (_solid)
                    {
                        Gizmos.DrawCube(box.center, box.size);
                    }
                    else
                    {
                        Gizmos.DrawWireCube(box.center, box.size);
                    }
                    break;
                case SphereCollider sphere:
                    float sphereRadius = sphere.radius * Mathf.Max(scale.x, scale.y, scale.z);
                    if (_solid)
                    {
                        Gizmos.DrawSphere(transform.TransformPoint(sphere.center), sphereRadius);
                    }
                    else
                    {
                        Gizmos.DrawWireSphere(transform.TransformPoint(sphere.center), sphereRadius);
                    }
                    break;
                case CapsuleCollider capsule:
                    DrawCapsule(capsule, scale);
                    break;
                default:
                    Bounds bounds = _collider.bounds;
                    if (_solid)
                    {
                        Gizmos.DrawCube(bounds.center, bounds.size);
                    }
                    else
                    {
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                    }
                    break;
            }

            Gizmos.matrix = previousMatrix;
        }


        private static void DrawCapsule(CapsuleCollider _capsule, Vector3 _scale)
        {
            Transform transform = _capsule.transform;
            Vector3 localAxis = _capsule.direction == 0 ? Vector3.right : _capsule.direction == 1 ? Vector3.up : Vector3.forward;
            Vector3 localSide = _capsule.direction == 0 ? Vector3.up : Vector3.right;
            float axisScale = _capsule.direction == 0 ? _scale.x : _capsule.direction == 1 ? _scale.y : _scale.z;
            float radiusScale = _capsule.direction == 0 ? Mathf.Max(_scale.y, _scale.z)
                : _capsule.direction == 1 ? Mathf.Max(_scale.x, _scale.z)
                : Mathf.Max(_scale.x, _scale.y);

            float radius = _capsule.radius * radiusScale;
            float halfSpan = Mathf.Max((_capsule.height * axisScale * 0.5f) - radius, 0f);
            Vector3 center = transform.TransformPoint(_capsule.center);
            Vector3 axis = transform.rotation * localAxis;
            Vector3 sideA = transform.rotation * localSide;
            Vector3 sideB = Vector3.Cross(axis, sideA);
            Vector3 top = center + (axis * halfSpan);
            Vector3 bottom = center - (axis * halfSpan);

            Gizmos.DrawWireSphere(top, radius);
            Gizmos.DrawWireSphere(bottom, radius);
            Gizmos.DrawLine(top + (sideA * radius), bottom + (sideA * radius));
            Gizmos.DrawLine(top - (sideA * radius), bottom - (sideA * radius));
            Gizmos.DrawLine(top + (sideB * radius), bottom + (sideB * radius));
            Gizmos.DrawLine(top - (sideB * radius), bottom - (sideB * radius));
        }
    }
} // Minikit namespace
