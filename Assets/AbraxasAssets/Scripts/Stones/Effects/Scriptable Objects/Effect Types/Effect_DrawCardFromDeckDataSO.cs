using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect DrawCardFromLibrary", menuName = "Abraxas/StoneData/Effects/Draw Card From Library")]
    [Serializable]
    public class Effect_DrawCardFromDeckDataSO : EffectSO
    {
        #region Fields
        [SerializeField]
        BasicStoneData _data = new();
        [SerializeField]
        TargetSO<Player> _target;
        #endregion

        #region Properties
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_TargetPlayerDrawsCardsFromDeck);
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Effect_DrawCardFromDeckDataSO))]
    public class Effect_DrawCardFromDeckDataSOEditor : EffectSOEditor
    {
        private SerializedProperty _targetProperty;

        public override void OnEnable()
        {
            base.OnEnable();
            _targetProperty = serializedObject.FindProperty("_target");
        }

        protected override void DrawCustomFields()
        {
            EditorGUILayout.PropertyField(_targetProperty);

            if (GUILayout.Button("Set Target"))
            {
                ShowSelectTypeMenu<TargetSO<Player>>(type => SetTarget(type, _targetProperty));
            }

            if (_targetProperty.objectReferenceValue != null && GUILayout.Button("Remove Target"))
            {
                RemoveTarget(_targetProperty);
            }
        }
    }
#endif
}
