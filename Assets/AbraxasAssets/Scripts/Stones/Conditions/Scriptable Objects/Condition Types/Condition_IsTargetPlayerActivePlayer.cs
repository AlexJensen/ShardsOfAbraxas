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
            Player = ((Condition_IsTargetPlayerActivePlayer)condition).Player;
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
        PlayerType _player;

        [SerializeField]
        TargetSO<Players.Players> _target;
        #endregion

        #region Properties
        public PlayerType Player { get => _player; set => _player = value; }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            var player = _target.Target;

            if (Player == PlayerType.Active
            ? player == _playerManager.ActivePlayer
            : player != _playerManager.ActivePlayer)
            {
                return false;
            }
            return true;
        }

        public override bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData)
        {
            return eventData.Data != _target.Target;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Condition_IsTargetPlayerActivePlayer))]
    public class IsTargetPlayerActivePlayerConditionEditor : StoneSOEditor
    {
        private SerializedProperty _playerProperty;
        private SerializedProperty _targetProperty;

        protected void OnEnable()
        {
            _playerProperty = serializedObject.FindProperty("_player");
            _targetProperty = serializedObject.FindProperty("_target");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_playerProperty);
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