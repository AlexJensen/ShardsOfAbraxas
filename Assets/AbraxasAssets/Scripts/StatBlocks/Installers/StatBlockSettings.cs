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
            public List<StatBlockCombination> statBlocks;

            [Serializable]
            public struct StatBlockCombination
            {
                [HideInInspector]
                public string Title;
                public StatData Stats;
                public Sprite Sprite;        
            }

            public Sprite GetSprite(StatData stats)
            {
                foreach (var combination in statBlocks)
                {
                    if (combination.Stats.Equals(stats))
                    {
                        return combination.Sprite;
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
            string displayName = $"{statProperty.FindPropertyRelative("ATK").intValue} " +
                                 $"{statProperty.FindPropertyRelative("DEF").intValue} " +
                                 $"{statProperty.FindPropertyRelative("SPD").intValue} " +
                                 $"{statProperty.FindPropertyRelative("RNG").intValue}";
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
