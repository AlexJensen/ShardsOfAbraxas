using Abraxas.Cards.Controllers;
using Abraxas.Stones.Data;
using UnityEditor;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target OwnerOfCard", menuName = "Abraxas/Data/StoneData/Targets/OwnerOfCard")]
    class Target_OwnerOfTargetCardSO : TargetSO<Player>
    {
        public TargetSO<ICardController> _target;
        public override Player Target
        {
            get
            {
                return _target.Target.Owner;
            }
            set
            {
                //
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Target_OwnerOfTargetCardSO))]
        public class Target_OwnerOfTargetCardEditor : StoneSOEditor
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
}
