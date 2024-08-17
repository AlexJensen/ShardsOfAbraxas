using Abraxas.Events;
using Abraxas.Players.Managers;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    [Serializable]
    [CreateAssetMenu(menuName = "Abraxas/Conditions/IsActivePlayer")]
    class Condition_IsTargetPlayerActivePlayer : ConditionSO<Event_ActivePlayerChanged>
    {
        #region Dependencies
        [Inject]
        protected readonly IPlayerManager _playerManager;

        public override void Initialize(IStoneController stone, ICondition condition)
        {
            Type = ((Condition_IsTargetPlayerActivePlayer)condition).Type;
            base.Initialize(stone, condition);
        }
        #endregion

        #region Enums
        public enum PlayerType
        {
            Active,
            Inactive
        }
        #endregion

        #region Fields
        [SerializeField]
        PlayerType _type;

        [SerializeField]
        TargetSO<Players.Players> _target;
        #endregion

        #region Properties
        public PlayerType Type { get => _type; set => _type = value; }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            var player = _target.Target;

            if (Type == PlayerType.Active
            ? player == _playerManager.ActivePlayer
            : player != _playerManager.ActivePlayer)
            {
                return false;
            }
            return true;
        }

        public override bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData)
        {
            return eventData.Player != _target.Target;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Condition_IsTargetPlayerActivePlayer))]
    public class IsTargetPlayerActivePlayerConditionEditor : StoneSOEditor
    {
        private SerializedProperty _typeProperty;
        private SerializedProperty _targetProperty;
        private SerializedProperty _isTriggerProperty;

        protected void OnEnable()
        {
            _typeProperty = serializedObject.FindProperty("_type");
            _targetProperty = serializedObject.FindProperty("_target");
            _isTriggerProperty = serializedObject.FindProperty("_isTrigger");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_typeProperty);
            EditorGUILayout.PropertyField(_targetProperty);
            EditorGUILayout.PropertyField(_isTriggerProperty);

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