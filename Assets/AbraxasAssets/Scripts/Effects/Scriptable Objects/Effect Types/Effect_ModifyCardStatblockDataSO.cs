using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect ModifyTargetCardGroupStatBlock", menuName = "Abraxas/Data/StoneData/Effects/Modify Target Card Group Stat Block")]
    [Serializable]
    public class Effect_ModifyCardGroupStatblockDataSO : EffectStoneSO
    {
        #region Fields
        [SerializeField]
        ModifyCardStatBlockData _data = new();
        [SerializeField]
        TargetSOBase _targets;
        #endregion

        #region Properties
        public override IStoneData Data { get => _data; set => _data = (ModifyCardStatBlockData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_ModifyTargetCardGroupStatblock);
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Effect_ModifyCardGroupStatblockDataSO))]
    public class Effect_ModifyCardGroupStatblockDataSOEditor : EffectSOEditor
    {
        private SerializedProperty _targetProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _targetProperty = serializedObject.FindProperty("_targets");
        }

        protected override void DrawCustomFields()
        {
            EditorGUILayout.PropertyField(_targetProperty);

            if (GUILayout.Button("Set Target"))
            {
                ShowSelectTypeMenu<TargetSOBase>(typeof(TargetSOBase), type => SetTarget(type, _targetProperty));
            }

            if (_targetProperty.objectReferenceValue != null && GUILayout.Button("Remove Target"))
            {
                RemoveTarget(_targetProperty);
            }
        }
    }
#endif
}
