using Abraxas.Stones.Data;
using UnityEditor;

namespace Abraxas.Passives
{
    public abstract class PassiveStoneSO : StoneSO
    {

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(PassiveStoneSO), true)]
    public class PassiveSOEditor : StoneSOEditor
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
