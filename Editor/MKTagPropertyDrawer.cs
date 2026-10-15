using UnityEditor;
using UnityEngine;

namespace Minikit.Editor
{
    [CustomPropertyDrawer(typeof(MKTag))]
    public class MKTagPropertyDrawer : PropertyDrawer
    {
        private static readonly Color unregisteredColor = new(1f, 0.55f, 0.45f);


        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty keyProperty = _property.FindPropertyRelative("key");

            EditorGUI.BeginProperty(_position, _label, _property);
            Rect buttonRect = EditorGUI.PrefixLabel(_position, _label);

            string key = keyProperty.stringValue;
            bool mixed = keyProperty.hasMultipleDifferentValues;
            bool unregistered = !mixed
                && !string.IsNullOrEmpty(key)
                && !MKTagRegistry.IsRegistered(key);

            string text = mixed ? "—" : string.IsNullOrEmpty(key) ? "None" : key;
            if (unregistered)
            {
                text += " (unregistered)";
            }

            Color previousColor = GUI.backgroundColor;
            if (unregistered)
            {
                GUI.backgroundColor = unregisteredColor;
            }

            bool clicked = EditorGUI.DropdownButton(buttonRect, new GUIContent(text, text), FocusType.Keyboard);
            GUI.backgroundColor = previousColor;

            if (clicked)
            {
                SerializedObject serializedObject = _property.serializedObject;
                string path = _property.propertyPath;
                MKTagDropdown.Show(buttonRect, mixed ? null : new[] { key }, false, _key => SetKey(serializedObject, path, _key));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private static void SetKey(SerializedObject _serializedObject, string _path, string _key)
        {
            if (_serializedObject == null
                || _serializedObject.targetObject == null)
            {
                return;
            }

            _serializedObject.Update();
            SerializedProperty keyProperty = _serializedObject.FindProperty(_path)?.FindPropertyRelative("key");
            if (keyProperty == null)
            {
                return;
            }

            keyProperty.stringValue = _key;
            _serializedObject.ApplyModifiedProperties();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
} // Minikit.Editor namespace
