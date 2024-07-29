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
    public class Condition_IsTargetEqualToTarget : ConditionSO<IEventBase>
    {
        #region Fields
        [SerializeField]
        private TargetSO<object> _firstTarget;
        [SerializeField]
        private TargetSO<object> _secondTarget;
        #endregion

        #region Methods
        public override bool IsMet()
        {
            return _firstTarget.Target.Equals(_secondTarget.Target);
        }

        public override bool ShouldReceiveEvent(IEventBase eventData)
        {
            if (_firstTarget is IGameEventListener<IEventBase> listener1)
            {
                listener1.OnEventRaised(eventData);
            }
            if (_secondTarget is IGameEventListener<IEventBase> listener2)
            {
                listener2.OnEventRaised(eventData);
            }
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
                    ShowSelectTypeMenu<ITarget>(type => SetTarget(type, _firstTargetProperty));
                }

                if (_firstTargetProperty.objectReferenceValue != null && GUILayout.Button("Remove First Target"))
                {
                    RemoveTarget(_firstTargetProperty);
                }

                if (GUILayout.Button("Set Second Target"))
                {
                    ShowSelectTypeMenu<ITarget>(type => SetTarget(type, _secondTargetProperty));
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
