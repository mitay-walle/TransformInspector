/// Custom transform inspector for Unity 2018.1.7f
///MIT License
///Copyright (c) 2018 mitay-walle

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.TransformInspector.Editor
{
    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class TransformEditor : UnityEditor.Editor
    {
        private bool EnableFinished;

        void OnEnable()
        {
            if (!EnableFinished)
            {
                EnableFinished = true;
                serializedObject.Update();
                serializedObject.FindProperty("m_LocalEulerAnglesHint").vector3Value =
                    serializedObject.FindProperty("m_LocalRotation").quaternionValue.eulerAngles;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUI.indentLevel = 0;
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60f;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(20f)))
            {
                Pos(serializedObject);
                return;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_LocalPosition"), new GUIContent(""));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(20f)))
            {
                Rot(serializedObject);
                return;
            }

            var prop2 = serializedObject.FindProperty("m_LocalRotation");

            RotationPropertyField(prop2, new GUIContent(""));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(20f)))
            {
                Sc(serializedObject);
                return;
            }


            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_LocalScale"), new GUIContent(""));

            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = oldWidth;


            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }

        void Pos(SerializedObject s)
        {
            s.FindProperty("m_LocalPosition").vector3Value = Vector3.zero;
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }

        void Rot(SerializedObject s)
        {
            s.FindProperty("m_LocalRotation").quaternionValue = Quaternion.Euler(Vector3.zero);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }

        void Sc(SerializedObject s)
        {
            s.FindProperty("m_LocalScale").vector3Value = Vector3.one;
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }


        private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
        {
            Transform transform = (Transform)targets[0];
            Quaternion localRotation = transform.localRotation;
            foreach (Object t in targets)
                if (!SameRotation(localRotation, ((Transform)t).localRotation))
                {
                    EditorGUI.showMixedValue = true;
                    break;
                }

            EditorGUI.BeginChangeCheck();

            Vector3 eulerAngles =
                EditorGUILayout.Vector3Field(content.text, TransformUtils.GetInspectorRotation(transform));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Rotation Changed");
                foreach (Object obj in targets)
                {
                    Transform t = (Transform)obj;
                    eulerAngles = FixIfNaN(eulerAngles);
                    TransformUtils.SetInspectorRotation(t, eulerAngles);
                }

                rotationProperty.serializedObject.SetIsDifferentCacheDirty();
            }

            EditorGUI.showMixedValue = false;
        }

        private Vector3 FixIfNaN(Vector3 v)
        {
            if (float.IsNaN(v.x)) v.x = 0;
            if (float.IsNaN(v.y)) v.y = 0;
            if (float.IsNaN(v.z)) v.z = 0;
            return v;
        }

        private bool SameRotation(Quaternion rot1, Quaternion rot2)
        {
            if (rot1.x != rot2.x) return false;
            if (rot1.y != rot2.y) return false;
            if (rot1.z != rot2.z) return false;
            if (rot1.w != rot2.w) return false;
            return true;
        }
    }
}
