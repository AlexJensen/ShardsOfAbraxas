using Abraxas.Cards.Controllers;
using Abraxas.Events;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using Abraxas.Zones;
using System;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/WasThisCardInZonePreviously")]
    [Serializable]
    public class Condition_WasTargetCardInZone : ConditionSO<Event_CardChangedZones>
    {
        #region Dependencies
        public override void Initialize(IStoneController stone, ICondition condition)
        {
            Zone = ((Condition_WasTargetCardInZone)condition).Zone;
            base.Initialize(stone, condition);
        }
        #endregion

        #region Fields
        [SerializeField]
        ZoneType _zone;
        [SerializeField]
        TargetSO<ICardController> _target;
        #endregion

        #region Properties
        public ZoneType Zone { get => _zone; set => _zone = value; }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            if (Stone.Card.PreviousZone == null) return false;
            return Zone == _target.Target.PreviousZone.Type;
        }

        public override bool ShouldReceiveEvent(Event_CardChangedZones eventData)
        {
            return IsMet();
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Condition_WasTargetCardInZone))]
    public class Condition_WasTargetCardInZoneEditor : StoneSOEditor
    {
        private SerializedProperty _zoneProperty;
        private SerializedProperty _targetProperty;

        protected void OnEnable()
        {
            _zoneProperty = serializedObject.FindProperty("_zone");
            _targetProperty = serializedObject.FindProperty("_target");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_zoneProperty);
            EditorGUILayout.PropertyField(_targetProperty);

            if (GUILayout.Button("Set Target"))
            {
                ShowSelectTypeMenu<TargetSO<ICardController>>(type => SetTarget(type, _targetProperty));
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