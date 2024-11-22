using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Data/StoneData/Conditions/TargetEqualsTarget")]
    [Serializable]
    public class Condition_IsTargetEqualToTarget : ConditionSO
    {
        #region Fields
        [SerializeField]
        private TargetSOBase _firstTarget;
        [SerializeField]
        private TargetSOBase _secondTarget;

        public override void Initialize(IStoneController stoneController, DiContainer container)
        {
            _firstTarget.Initialize(stoneController);
            _secondTarget.Initialize(stoneController);
        }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            return _firstTarget.GetTarget().Equals(_secondTarget.GetTarget());
        }

        internal override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            throw new NotImplementedException();
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
