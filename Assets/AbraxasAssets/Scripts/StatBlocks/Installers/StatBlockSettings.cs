using System.Collections.Generic;
using System;
using UnityEngine;
using Abraxas.StatBlocks.Data;
using UnityEditor;

namespace Abraxas.StatBlocks
{

    public static class Statblock
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            public StatData StatMultiplier;
            public List<StatBlockCombination> statBlocks;

            [Serializable]
            public struct StatBlockCombination
            {
                [HideInInspector]
                public string Title;
                public StatData Stats;
                public Sprite Sprite;
                public AnimationClip AttackAnimation;
            }

            public Sprite GetSprite(StatData stats)
            {
                StatData adjustedStats = stats / StatMultiplier;
                foreach (var combination in statBlocks)
                {
                    if (combination.Stats.Equals(adjustedStats))
                    {
                        return combination.Sprite;
                    }
                }
                return null;
            }

            public AnimationClip GetAttackAnimation(StatData stats)
            {
                StatData adjustedStats = stats / StatMultiplier;
                foreach (var combination in statBlocks)
                {
                    if (combination.Stats.Equals(adjustedStats))
                    {
                        return combination.AttackAnimation;
                    }
                }
                return null;
            }
        }
        #endregion
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Statblock.Settings.StatBlockCombination))]
    public class StatBlockCombinationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get properties
            SerializedProperty titleProperty = property.FindPropertyRelative("Title");
            SerializedProperty statProperty = property.FindPropertyRelative("Stats");

            // Generate display name
            string displayName = $"{statProperty.FindPropertyRelative("_ATK").intValue} " +
                                 $"{statProperty.FindPropertyRelative("_DEF").intValue} " +
                                 $"{statProperty.FindPropertyRelative("_SPD").intValue} " +
                                 $"{statProperty.FindPropertyRelative("_RNG").intValue}";
            titleProperty.stringValue = displayName;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
    #endif
}
