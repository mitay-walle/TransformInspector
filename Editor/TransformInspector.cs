using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.UI.Editor
{
    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class TransformInspector : UnityEditor.Editor
    {
        private const int BUTTON_INDENT = 20;
        private const int BUTTON_WIDTH = 20;
        private const int BUTTON_HEIGHT = 18;
        private const int BUTTON_HEIGHT_SPACING = 2;
        private const int BUTTON_Y_START = 4;
        private const int INDENT_LEVEL = 2;

        private UnityEditor.Editor _builtInEditor;

        protected void OnEnable()
        {
            _builtInEditor = CreateEditor(targets,
                typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TransformInspector"));
        }

        protected void OnDisable()
        {
            OnDestroy();
        }

        private void OnDestroy()
        {
            if (_builtInEditor) DestroyImmediate(_builtInEditor);
        }

        public override void OnInspectorGUI()
        {
            if (!_builtInEditor)
            {
                OnEnable();
                return;
            }

            if (targets.Length != _builtInEditor.targets.Length)
            {
                OnEnable();
                return;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != _builtInEditor.targets[i])
                {
                    OnEnable();
                    return;
                }
            }

            var old = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;
            EditorGUI.indentLevel += INDENT_LEVEL;
            _builtInEditor.OnInspectorGUI();
            EditorGUI.indentLevel -= INDENT_LEVEL;
            EditorGUIUtility.labelWidth = old;

            var buttonRect = new Rect(BUTTON_INDENT, BUTTON_Y_START, BUTTON_WIDTH, BUTTON_HEIGHT);

            if (GUI.Button(buttonRect, "X")) //, GUILayout.Width(20f)))
            {
                ResetPosition();
                return;
            }

            buttonRect = new Rect(BUTTON_INDENT,
                BUTTON_Y_START + BUTTON_HEIGHT_SPACING + BUTTON_HEIGHT,
                BUTTON_WIDTH,
                BUTTON_HEIGHT);

            if (GUI.Button(buttonRect, "X")) //, GUILayout.Width(20f)))
            {
                ResetRotation();
                return;
            }

            buttonRect = new Rect(BUTTON_INDENT,
                BUTTON_Y_START + 2 * (BUTTON_HEIGHT + BUTTON_HEIGHT_SPACING),
                BUTTON_WIDTH,
                BUTTON_HEIGHT);

            if (GUI.Button(buttonRect, "X")) //, GUILayout.Width(20f)))
            {
                ResetScale();
                return;
            }
        }

        void ResetPosition()
        {
            for (int i = 0; i < _builtInEditor.targets.Length; i++)
            {
                var transform = _builtInEditor.targets[i] as Transform;
                if (transform.localPosition != Vector3.zero)
                {
                    Undo.RecordObject(transform, "Position reset");
                    transform.localPosition = Vector3.zero;
                }
            }

            OnEnable();
        }

        void ResetRotation()
        {
            var rotation = Quaternion.Euler(Vector3.zero);
            for (int i = 0; i < _builtInEditor.targets.Length; i++)
            {
                var transform = _builtInEditor.targets[i] as Transform;
                if (transform.localRotation != Quaternion.identity)
                {
                    Undo.RecordObject(transform, "Rotation reset");
                    transform.localRotation = rotation;
                }
            }

            OnEnable();
        }

        void ResetScale()
        {
            for (int i = 0; i < _builtInEditor.targets.Length; i++)
            {
                var transform = _builtInEditor.targets[i] as Transform;
                if (transform.localScale != Vector3.one)
                {
                    Undo.RecordObject(transform, "Scale reset");
                    transform.localScale = Vector3.one;
                }
            }

            OnEnable();
        }
    }
}
