using Abraxas.Events;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Data/StoneData/Conditions/TargetPlayerDrawsCard")]
    [Serializable]
    public class Condition_Trigger_PlayerDrawsCards : ConditionSO<Event_PlayerDrawsCards>
    {
        #region Dependencies
        public override void Initialize(IStoneController stone, ICondition condition, DiContainer container)
        {
            base.Initialize(stone, condition, container);
        }
        #endregion

        public override bool IsTrigger => true;

        #region Fields
        [SerializeField]
        TargetSO<Players.Players> _target;
        #endregion



        #region Methods
        public override bool IsMet()
        {
            return true;
        }

        public override bool ShouldReceiveEvent(Event_PlayerDrawsCards eventData)
        {
            return IsMet();
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Condition_Trigger_PlayerDrawsCards))]
    public class Condition_Trigger_PlayerDrawsCardsEditor : StoneSOEditor
    {
        private SerializedProperty _targetProperty;

        protected void OnEnable()
        {
            _targetProperty = serializedObject.FindProperty("_target");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_targetProperty);

            if (GUILayout.Button("Set Target"))
            {
                ShowSelectTypeMenu<TargetSO<Players.Players>>(type => SetTarget(type, _targetProperty));
            }

            if (_targetProperty.objectReferenceValue != null && GUILayout.Button("Remove Target"))
            {
                RemoveTarget(_targetProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}