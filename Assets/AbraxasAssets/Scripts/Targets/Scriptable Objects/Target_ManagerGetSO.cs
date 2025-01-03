using Abraxas.Core;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Targets
{
    /// <summary>
    /// Target_ManagerGetSO is a stone target that returns a chosen manager instance (an IManager).
    /// The user picks which manager type in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "New Target GetManager",
                     menuName = "Abraxas/Data/StoneData/Targets/GetManager")]
    class Target_ManagerGetSO : TargetSO<IManager>
    {
        [SerializeField, HideInInspector]
        private string _managerTypeName;

        // This is how we actually fetch the manager at runtime.
        // If you prefer a different aggregator approach, adapt accordingly.
        [Inject(Optional = true)]
        private readonly IManager[] _allManagers = null;


        public override Type ResolveRuntimeType()
        {
            if (string.IsNullOrEmpty(_managerTypeName))
                return typeof(IManager);

            var managerType = Type.GetType(_managerTypeName);
            return managerType ?? typeof(IManager);
        }

        public override IManager Target
        {
            get
            {
                if (string.IsNullOrEmpty(_managerTypeName))
                {
                    Debug.LogWarning($"{name}: No manager type selected.");
                    return null;
                }

                var managerType = Type.GetType(_managerTypeName);
                if (managerType == null)
                {
                    Debug.LogWarning($"{name}: Could not find type '{_managerTypeName}'.");
                    return null;
                }

                if (_allManagers == null)
                {
                    Debug.LogWarning($"{name}: No managers array was injected. " +
                                     "Cannot find manager of type '{managerType.Name}'.");
                    return null;
                }

                var mgrInstance = _allManagers.FirstOrDefault(m => m.GetType() == managerType);
                if (mgrInstance == null)
                {
                    Debug.LogWarning($"{name}: Could not find any manager of type {managerType.Name} in the scene?");
                }

                return mgrInstance;
            }
            set
            {
                // We generally do not override the manager reference at runtime
            }
        }

        /// <summary>
        /// Called by custom inspector to record which manager type is chosen.
        /// </summary>
        public void SetManagerType(Type managerType)
        {
            if (managerType != null && typeof(IManager).IsAssignableFrom(managerType))
            {
                _managerTypeName = managerType.AssemblyQualifiedName;
            }
            else
            {
                _managerTypeName = "";
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Target_ManagerGetSO))]
    public class Target_ManagerGetSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Show the field if you want to see the raw type string (optional)
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_managerTypeName"));
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Select Manager Type"))
            {
                ShowManagerTypeMenu();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowManagerTypeMenu()
        {
            var menu = new GenericMenu();

            // Look up all types that implement IManager
            var managerInterface = typeof(IManager);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Collect concrete manager classes
            var managerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => managerInterface.IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract)
                .ToList();

            if (managerTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Manager types found"));
            }
            else
            {
                foreach (var mgrType in managerTypes)
                {
                    var localType = mgrType;
                    menu.AddItem(new GUIContent(localType.Name), false, () =>
                    {
                        // We set the type in the script's data
                        var managerGet = (Target_ManagerGetSO)target;
                        managerGet.SetManagerType(localType);
                        EditorUtility.SetDirty(managerGet);
                    });
                }
            }

            menu.ShowAsContext();
        }
    }

#endif

}
