using Abraxas.Events;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/Triggers/TargetPlayerDrawsCard")]
    [Serializable]
    public class Condition_Trigger_PlayerDrawsCards : ConditionSO<Event_PlayerDrawsCards>
    {
        #region Dependencies
        public override void Initialize(IStoneController stone, ICondition condition)
        {
            base.Initialize(stone, condition);
        }
        #endregion

        #region Fields
        [SerializeField]
        TargetSO<Players.Players> _target;

        [HideInInspector]
        public new bool IsTrigger = true;
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