using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Abraxas
{
    [ExecuteInEditMode]
    public class ManaPercentageView : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Color[] _colors = new Color[12];

        [SerializeField, Range(0f, 1f)]
        private float[] _segments = new float[12];
        #endregion

        #region Methods
        private void OnValidate()
        {
            // Ensure all segments sum to 1
            NormalizeSegments();
        }

        private void NormalizeSegments()
        {
            float total = _segments.Sum();
            if (total == 0)
                return;

            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i] /= total;
            }
        }

        void Update()
        {
            Material mat = GetComponent<Image>().material;

            // Assign colors as Vector4 since _TintColors should be used with SetVectorArray
            Vector4[] colors = new Vector4[_colors.Length];
            for (int i = 0; i < _colors.Length; i++)
            {
                colors[i] = _colors[i];
            }
            mat.SetVectorArray("_TintColors", colors);

            // Assign segments
            mat.SetFloatArray("_Segments", _segments);
        }
        #endregion

#if UNITY_EDITOR
        // Custom Editor to modify the sliders in the inspector
        [UnityEditor.CustomEditor(typeof(ManaPercentageView))]
        public class ManaPercentageViewEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                ManaPercentageView script = (ManaPercentageView)target;

                serializedObject.Update();

                // Colors array
                UnityEditor.SerializedProperty colorsProperty = serializedObject.FindProperty("_colors");
                UnityEditor.EditorGUILayout.PropertyField(colorsProperty, true);

                // Segments array with sliders
                UnityEditor.SerializedProperty segmentsProperty = serializedObject.FindProperty("_segments");
                for (int i = 0; i < segmentsProperty.arraySize; i++)
                {
                    float currentSegment = segmentsProperty.GetArrayElementAtIndex(i).floatValue;
                    float newSegment = UnityEditor.EditorGUILayout.Slider($"Segment {i + 1}", currentSegment, 0f, 1f);
                    if (currentSegment != newSegment)
                    {
                        segmentsProperty.GetArrayElementAtIndex(i).floatValue = newSegment;
                        serializedObject.ApplyModifiedProperties();
                        script.NormalizeSegments();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
