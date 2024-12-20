using Abraxas.Players;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect DrawCardFromLibrary", menuName = "Abraxas/Data/StoneData/Effects/Draw Card From Library")]
    [Serializable]
    public class Effect_DrawCardFromDeckDataSO : EffectStoneSO
    {
        #region Fields
        [SerializeField]
        BasicStoneData _data = new();
        [SerializeField]
        TargetSOBase _target;
        #endregion

        #region Properties
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_DrawCardsFromDeck);
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Effect_DrawCardFromDeckDataSO))]
    public class Effect_DrawCardFromDeckDataSOEditor : EffectSOEditor
    {
        private SerializedProperty _targetProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _targetProperty = serializedObject.FindProperty("_target");
        }

        protected override void DrawCustomFields()
        {
            EditorGUILayout.PropertyField(_targetProperty);

            // Suppose we want to derive expectedType from the effect
            StoneSO effect = (StoneSO)target;
            Type expectedType = GetExpectedTypeFromEffect(effect);

            if (GUILayout.Button("Set Target"))
            {
                ShowSelectTypeMenu<TargetSOBase>(expectedType, t => SetTarget(t, _targetProperty, expectedType));
            }

            if (_targetProperty.objectReferenceValue != null && GUILayout.Button("Remove Target"))
            {
                RemoveTarget(_targetProperty);
            }
        }
    }
#endif
}
