using Abraxas.Events;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/TargetEqualsTarget")]
    [Serializable]
    public class Condition_IsTargetEqualToTarget : ConditionSO<IEvent>
    {
        #region Fields
        [SerializeField]
        private TargetSOBase _firstTarget;
        [SerializeField]
        private TargetSOBase _secondTarget;
        #endregion

        #region Methods
        public override bool IsMet()
        {
            return _firstTarget.GetTarget().Equals(_secondTarget.GetTarget());
        }

        public override bool ShouldReceiveEvent(IEvent eventData)
        {
            return true;
        }
        #endregion

#if UNITY_EDITOR
        [CustomEditor(typeof(Condition_IsTargetEqualToTarget))]
        public class IsTargetEqualToTargetEditor : StoneSOEditor
        {
            private SerializedProperty _firstTargetProperty;
            private SerializedProperty _secondTargetProperty;

            protected void OnEnable()
            {
                _firstTargetProperty = serializedObject.FindProperty("_firstTarget");
                _secondTargetProperty = serializedObject.FindProperty("_secondTarget");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(_firstTargetProperty);
                EditorGUILayout.PropertyField(_secondTargetProperty);

                if (GUILayout.Button("Set First Target"))
                {
                    ShowSelectTypeMenu<TargetSOBase>(type => SetTarget(type, _firstTargetProperty));
                }

                if (_firstTargetProperty.objectReferenceValue != null && GUILayout.Button("Remove First Target"))
                {
                    RemoveTarget(_firstTargetProperty);
                }

                if (GUILayout.Button("Set Second Target"))
                {
                    ShowSelectTypeMenu<TargetSOBase>(type => SetTarget(type, _secondTargetProperty));
                }

                if (_secondTargetProperty.objectReferenceValue != null && GUILayout.Button("Remove Second Target"))
                {
                    RemoveTarget(_secondTargetProperty);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
