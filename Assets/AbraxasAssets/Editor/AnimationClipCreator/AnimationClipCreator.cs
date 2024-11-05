using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class AnimationClipCreator : EditorWindow
{
    // Fields for the user input
    private Texture2D spriteSheet;
    private int startFrame = 0;
    private int endFrame = 0;
    private int frameStep = 1;
    private float frameRate = 12f; // Frames per second
    private string animationName = "NewAnimation";

    [MenuItem("Tools/Animation Clip Creator")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipCreator>("Animation Clip Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Animation Clip from Sprite Sheet (UI Images)", EditorStyles.boldLabel);

        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        animationName = EditorGUILayout.TextField("Animation Name", animationName);
        frameRate = EditorGUILayout.FloatField("Frame Rate", frameRate);
        startFrame = EditorGUILayout.IntField("Start Frame Index", startFrame);
        endFrame = EditorGUILayout.IntField("End Frame Index", endFrame);
        frameStep = EditorGUILayout.IntField("Frame Step", frameStep);

        if (GUILayout.Button("Create Animation Clip"))
        {
            CreateAnimationClip();
        }
    }

    private void CreateAnimationClip()
    {
        if (spriteSheet == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a sprite sheet.", "OK");
            return;
        }

        string spriteSheetPath = AssetDatabase.GetAssetPath(spriteSheet);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);
        List<Sprite> sprites = new();

        foreach (var asset in assets)
        {
            if (asset is Sprite)
            {
                sprites.Add(asset as Sprite);
            }
        }

        // Sort sprites using natural sort order
        sprites.Sort(CompareSpriteNames);

        // Validate frame indices
        if (startFrame < 0 || startFrame >= sprites.Count)
        {
            EditorUtility.DisplayDialog("Error", "Start Frame Index is out of bounds.", "OK");
            return;
        }

        if (endFrame < startFrame || endFrame >= sprites.Count)
        {
            EditorUtility.DisplayDialog("Error", "End Frame Index is out of bounds.", "OK");
            return;
        }

        if (frameStep <= 0)
        {
            EditorUtility.DisplayDialog("Error", "Frame Step must be greater than zero.", "OK");
            return;
        }

        // Collect the sprites to include in the animation clip
        List<ObjectReferenceKeyframe> keyFrames = new();

        int frameIndex = 0;
        for (int i = startFrame; i <= endFrame; i += frameStep)
        {
            var sprite = sprites[i];

            ObjectReferenceKeyframe keyFrame = new()
            {
                time = frameIndex / frameRate,
                value = sprite
            };
            keyFrames.Add(keyFrame);

            frameIndex++;
        }

        // Create the animation clip
        AnimationClip clip = new()
        {
            frameRate = frameRate
        };

        // Create the binding for the Image component's sprite property
        EditorCurveBinding spriteBinding = new()
        {
            type = typeof(Image), // Use UnityEngine.UI.Image
            path = "", // Empty if the Image component is on the same GameObject
            propertyName = "m_Sprite" // The internal name for the sprite property
        };

        // Assign the keyframes to the animation clip
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames.ToArray());

        // Set the clip to loop
        var serializedClip = new SerializedObject(clip);
        var settings = serializedClip.FindProperty("m_AnimationClipSettings");
        settings.FindPropertyRelative("m_LoopTime").boolValue = true;
        serializedClip.ApplyModifiedProperties();

        // Save the animation clip
        string spriteSheetDirectory = Path.GetDirectoryName(spriteSheetPath);
        string animationPath = Path.Combine(spriteSheetDirectory + "\\Animations", animationName + ".anim");

        AssetDatabase.CreateAsset(clip, animationPath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Success", "Animation clip created at:\n" + animationPath, "OK");
    }

    // Custom comparison function for natural sorting of sprite names
    private int CompareSpriteNames(Sprite a, Sprite b)
    {
        string pattern = @"_(\d+)$"; // Pattern to match the trailing number after an underscore

        int aIndex = ExtractNumberFromName(a.name, pattern);
        int bIndex = ExtractNumberFromName(b.name, pattern);

        return aIndex.CompareTo(bIndex);
    }

    private int ExtractNumberFromName(string name, string pattern)
    {
        Match match = Regex.Match(name, pattern);
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out int number))
            {
                return number;
            }
        }
        return 0; // Default to 0 if no number is found
    }
}
