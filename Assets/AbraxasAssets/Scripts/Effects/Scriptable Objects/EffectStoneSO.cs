﻿using System;
using UnityEditor;

namespace Abraxas.Stones.Data
{
    public abstract class EffectStoneSO : StoneSO
    {

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EffectStoneSO), true)]
    public class EffectSOEditor : StoneSOEditor
    {
        private SerializedProperty _dataProperty;

        protected virtual void OnEnable()
        {
            _dataProperty = serializedObject.FindProperty("_data");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_dataProperty);
            DrawCustomFields();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCustomFields()
        {
            // To be overridden by derived classes
        }
    }
#endif
}