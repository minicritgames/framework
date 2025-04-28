using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Minikit.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MKEditorButtonAttribute : PropertyAttribute
    {
    }

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MKEditorButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MonoBehaviour monoBehaviour = target as MonoBehaviour;
            if (monoBehaviour != null)
            {
                IEnumerable<MemberInfo> methods = monoBehaviour.GetType()
                    .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(mi => System.Attribute.IsDefined(mi, typeof(MKEditorButtonAttribute)));

                foreach (MemberInfo memberInfo in methods)
                {
                    if (GUILayout.Button(memberInfo.Name))
                    {
                        MethodInfo method = memberInfo as MethodInfo;
                        if (method != null)
                        {
                            method.Invoke(monoBehaviour, null);
                        }
                    }
                }
            }
        }
    }
} // Minikit.Editor namespace
