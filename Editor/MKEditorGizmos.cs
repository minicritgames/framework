using UnityEditor;
using UnityEngine;

namespace Minikit.Editor
{
    public static class MKEditorGizmos
    {
        /// <remarks> WARNING! Only usable within #if UNITY_EDITOR </remarks>
        public static void DrawText(Vector3 _position, string _text)
        {
    #if UNITY_EDITOR
            Handles.color = Gizmos.color;
            Handles.Label(_position, _text);
    #endif // UNITY_EDITOR
        }
    }
} // Minikit.Editor namespace
