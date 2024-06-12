using Abraxas.StatBlocks.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using static UnityEngine.GraphicsBuffer;

namespace Abraxas.StatBlocks.Installers
{
    /// <summary>
    /// CardSettingsInstaller is a Zenject installer for card settings.
    /// </summary>
    [CreateAssetMenu(fileName = "StatblockSettings", menuName = "Abraxas/Settings/Statblock Settings")]
    class StatBlockSettingsInstaller : ScriptableObjectInstaller<StatBlockSettingsInstaller>
    {
        public Statblock.Settings StatblockSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(StatblockSettings).AsSingle();
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(StatBlockSettingsInstaller))]
    public class StatblockSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StatBlockSettingsInstaller settings = (StatBlockSettingsInstaller)target;

            if (GUILayout.Button("Generate All Stat Block Combinations"))
            {
                GenerateAllCombinations(settings);
            }
        }

        private void GenerateAllCombinations(StatBlockSettingsInstaller settings)
        {
            settings.StatblockSettings.statBlocks = new List<Statblock.Settings.StatBlockCombination>();

            for (int rng = 0; rng <= 2; rng++)
            {
                for (int spd = 0; spd <= 2; spd++)
                {
                    for (int atk = 0; atk <= 5; atk++)
                    {
                        for (int def = 0; def <= 5; def++)
                        {
                            Statblock.Settings.StatBlockCombination combination = new()
                            {
                                Stats = new StatData
                                {
                                    ATK = atk,
                                    DEF = def,
                                    SPD = spd,
                                    RNG = rng
                                },
                                Title = $"{atk} {def} {spd} {rng}",
                                Sprite = null
                            };

                            settings.StatblockSettings.statBlocks.Add(combination);
                        }
                    }
                }
            }

            EditorUtility.SetDirty(settings);
        }
    }
    #endif
}
