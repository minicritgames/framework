using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minikit.Editor
{
    [CustomPropertyDrawer(typeof(MKTagContainer))]
    public class MKTagContainerPropertyDrawer : PropertyDrawer
    {
        private const float removeButtonWidth = 22f;
        private const float clearButtonWidth = 48f;

        private static readonly Color unregisteredColor = new(1f, 0.55f, 0.45f);


        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty tagsProperty = _property.FindPropertyRelative("tags");
            bool mixed = tagsProperty.hasMultipleDifferentValues;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginProperty(_position, _label, _property);

            Rect headerRect = new(_position.x, _position.y, _position.width, lineHeight);
            Rect fieldRect = EditorGUI.PrefixLabel(headerRect, _label);

            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            bool showClear = mixed || tagsProperty.arraySize > 0;
            Rect editRect = fieldRect;
            Rect clearRect = Rect.zero;
            if (showClear)
            {
                editRect.width -= clearButtonWidth + 2f;
                clearRect = new Rect(editRect.xMax + 2f, fieldRect.y, clearButtonWidth, fieldRect.height);
            }

            if (GUI.Button(editRect, "Edit Tags…", EditorStyles.miniButton))
            {
                List<string> current = new();
                if (!mixed)
                {
                    for (int i = 0; i < tagsProperty.arraySize; i++)
                    {
                        current.Add(tagsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue);
                    }
                }

                SerializedObject serializedObject = _property.serializedObject;
                string path = _property.propertyPath;
                MKTagDropdown.Show(editRect, current, true, _key => ToggleKey(serializedObject, path, _key));
            }

            bool cleared = showClear
                && GUI.Button(clearRect, "Clear", EditorStyles.miniButton);
            if (cleared)
            {
                tagsProperty.ClearArray();
                GUI.changed = true;
            }

            float y = headerRect.yMax + spacing;
            if (mixed
                && !cleared)
            {
                EditorGUI.LabelField(new Rect(fieldRect.x, y, fieldRect.width, lineHeight), "— (different values)");
            }
            else if (!cleared)
            {
                int removeIndex = -1;
                for (int i = 0; i < tagsProperty.arraySize; i++)
                {
                    string key = tagsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue;
                    bool unregistered = !MKTagRegistry.IsRegistered(key);

                    Rect labelRect = new(fieldRect.x, y, fieldRect.width - removeButtonWidth - 2f, lineHeight);
                    Rect removeRect = new(labelRect.xMax + 2f, y, removeButtonWidth, lineHeight);

                    Color previousColor = GUI.contentColor;
                    if (unregistered)
                    {
                        GUI.contentColor = unregisteredColor;
                    }

                    string text = unregistered ? (string.IsNullOrEmpty(key) ? "None" : key) + " (unregistered)" : key;
                    EditorGUI.LabelField(labelRect, new GUIContent(text, text));
                    GUI.contentColor = previousColor;

                    if (GUI.Button(removeRect, "×", EditorStyles.miniButton))
                    {
                        removeIndex = i;
                    }

                    y += lineHeight + spacing;
                }

                if (removeIndex >= 0)
                {
                    tagsProperty.DeleteArrayElementAtIndex(removeIndex);
                    GUI.changed = true;
                }
            }

            EditorGUI.indentLevel = previousIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty tagsProperty = _property.FindPropertyRelative("tags");
            int rows = tagsProperty.hasMultipleDifferentValues ? 1 : tagsProperty.arraySize;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return lineHeight + rows * (lineHeight + spacing);
        }

        private static void ToggleKey(SerializedObject _serializedObject, string _path, string _key)
        {
            if (_serializedObject == null
                || _serializedObject.targetObject == null
                || string.IsNullOrEmpty(_key))
            {
                return;
            }

            _serializedObject.Update();
            SerializedProperty tagsProperty = _serializedObject.FindProperty(_path)?.FindPropertyRelative("tags");
            if (tagsProperty == null)
            {
                return;
            }

            int existingIndex = -1;
            for (int i = 0; i < tagsProperty.arraySize; i++)
            {
                if (tagsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue == _key)
                {
                    existingIndex = i;
                    break;
                }
            }

            if (existingIndex >= 0)
            {
                tagsProperty.DeleteArrayElementAtIndex(existingIndex);
            }
            else
            {
                int newIndex = tagsProperty.arraySize;
                tagsProperty.InsertArrayElementAtIndex(newIndex);
                tagsProperty.GetArrayElementAtIndex(newIndex).FindPropertyRelative("key").stringValue = _key;
            }

            _serializedObject.ApplyModifiedProperties();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
} // Minikit.Editor namespace
