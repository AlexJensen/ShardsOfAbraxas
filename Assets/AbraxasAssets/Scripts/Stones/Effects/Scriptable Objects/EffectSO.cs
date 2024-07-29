using System;
using UnityEditor;

namespace Abraxas.Stones.Data
{
    public abstract class EffectSO : StoneSO
    {

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EffectSO), true)]
    public class EffectSOEditor : StoneSOEditor
    {
        private SerializedProperty _dataProperty;

        public virtual void OnEnable()
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