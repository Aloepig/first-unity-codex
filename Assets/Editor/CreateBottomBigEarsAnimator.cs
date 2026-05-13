using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;

public static class CreateCharacterAnimatorFromFolder
{
    [MenuItem("Assets/Animation/Create Character Animator From Folder", true)]
    private static bool ValidateCreateAnimatorAssetsFromSelection()
    {
        string folder = GetSelectedFolderPath();
        return !string.IsNullOrEmpty(folder) && folder.StartsWith("Assets/Art/");
    }

    [MenuItem("Assets/Animation/Create Character Animator From Folder")]
    private static void CreateAnimatorAssetsFromSelection()
    {
        string basePath = GetSelectedFolderPath();
        if (string.IsNullOrEmpty(basePath))
        {
            Debug.LogError("Select a character folder under Assets/Art first.");
            return;
        }

        CreateAnimatorAssets(basePath);
    }

    [MenuItem("Tools/Animation/Create Character Animator (Selected Art Folder)")]
    private static void CreateAnimatorAssetsFromToolsMenu()
    {
        string basePath = GetSelectedFolderPath();
        if (string.IsNullOrEmpty(basePath))
        {
            basePath = PromptFolderUnderAssetsArt();
            if (string.IsNullOrEmpty(basePath))
            {
                Debug.LogError("Select a character folder under Assets/Art first.");
                return;
            }
        }

        CreateAnimatorAssets(basePath);
    }

    private static void CreateAnimatorAssets(string basePath)
    {
        string idlePath = $"{basePath}/idle.png";
        string walk1Path = $"{basePath}/walk_1.png";
        string walk2Path = $"{basePath}/walk_2.png";

        string outputDir = $"{basePath}/Animator";
        string characterName = Path.GetFileName(basePath);
        string idleClipPath = $"{outputDir}/{characterName}_idle.anim";
        string walkClipPath = $"{outputDir}/{characterName}_walk.anim";
        string controllerPath = $"{outputDir}/{characterName}.controller";

        EnsureTextureSettings(idlePath);
        EnsureTextureSettings(walk1Path);
        EnsureTextureSettings(walk2Path);

        Sprite idle = AssetDatabase.LoadAssetAtPath<Sprite>(idlePath);
        Sprite walk1 = AssetDatabase.LoadAssetAtPath<Sprite>(walk1Path);
        Sprite walk2 = AssetDatabase.LoadAssetAtPath<Sprite>(walk2Path);

        if (idle == null || walk1 == null || walk2 == null)
        {
            Debug.LogError($"Sprite load failed in {basePath}. Required files: idle.png, walk_1.png, walk_2.png");
            return;
        }

        if (!AssetDatabase.IsValidFolder(outputDir))
        {
            AssetDatabase.CreateFolder(basePath, "Animator");
        }

        AnimationClip idleClip = CreateIdleClip(idle, idleClipPath);
        AnimationClip walkClip = CreateWalkClip(walk1, walk2, walkClipPath);
        AnimatorController controller = CreateController(idleClip, walkClip, controllerPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Animator assets for '{characterName}' at {outputDir}");
    }

    private static string GetSelectedFolderPath()
    {
        Object selected = Selection.activeObject;
        if (selected == null)
        {
            return null;
        }

        string path = AssetDatabase.GetAssetPath(selected);
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        if (AssetDatabase.IsValidFolder(path))
        {
            return path;
        }

        string directory = Path.GetDirectoryName(path)?.Replace("\\", "/");
        if (!string.IsNullOrEmpty(directory) && AssetDatabase.IsValidFolder(directory))
        {
            return directory;
        }

        return null;
    }

    private static string PromptFolderUnderAssetsArt()
    {
        string absolute = EditorUtility.OpenFolderPanel("Select Character Folder", Application.dataPath, "");
        if (string.IsNullOrEmpty(absolute))
        {
            return null;
        }

        string normalized = absolute.Replace("\\", "/");
        string assetsRoot = Application.dataPath.Replace("\\", "/");
        if (!normalized.StartsWith(assetsRoot))
        {
            Debug.LogError("Selected folder must be inside this project's Assets folder.");
            return null;
        }

        string relative = "Assets" + normalized.Substring(assetsRoot.Length);
        if (!relative.StartsWith("Assets/Art/"))
        {
            Debug.LogError("Selected folder must be under Assets/Art.");
            return null;
        }

        return relative;
    }

    private static void EnsureTextureSettings(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        bool dirty = false;
        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            dirty = true;
        }

        if (importer.spriteImportMode != SpriteImportMode.Single)
        {
            importer.spriteImportMode = SpriteImportMode.Single;
            dirty = true;
        }

        if (importer.filterMode != FilterMode.Point)
        {
            importer.filterMode = FilterMode.Point;
            dirty = true;
        }

        if (importer.alphaIsTransparency == false)
        {
            importer.alphaIsTransparency = true;
            dirty = true;
        }

        if (dirty)
        {
            importer.SaveAndReimport();
        }
    }

    private static AnimationClip CreateIdleClip(Sprite idle, string path)
    {
        AnimationClip clip = new AnimationClip { frameRate = 12f };
        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        ObjectReferenceKeyframe[] frames =
        {
            new ObjectReferenceKeyframe { time = 0f, value = idle },
            new ObjectReferenceKeyframe { time = 1f / 12f, value = idle }
        };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, frames);

        AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (existing != null)
        {
            EditorUtility.CopySerialized(clip, existing);
            return existing;
        }

        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static AnimationClip CreateWalkClip(Sprite walk1, Sprite walk2, string path)
    {
        AnimationClip clip = new AnimationClip { frameRate = 8f };
        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        ObjectReferenceKeyframe[] frames =
        {
            new ObjectReferenceKeyframe { time = 0f, value = walk1 },
            new ObjectReferenceKeyframe { time = 1f / 8f, value = walk2 },
            new ObjectReferenceKeyframe { time = 2f / 8f, value = walk1 }
        };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, frames);

        AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (existing != null)
        {
            EditorUtility.CopySerialized(clip, existing);
            return existing;
        }

        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static AnimatorController CreateController(AnimationClip idle, AnimationClip walk, string path)
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        }

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;

        // Clear previous states/transitions for deterministic output.
        sm.states = new ChildAnimatorState[0];
        sm.anyStateTransitions = new AnimatorStateTransition[0];
        sm.entryTransitions = new AnimatorTransition[0];

        bool hasSpeed = false;
        foreach (AnimatorControllerParameter p in controller.parameters)
        {
            if (p.name == "Speed" && p.type == AnimatorControllerParameterType.Float)
            {
                hasSpeed = true;
                break;
            }
        }

        if (!hasSpeed)
        {
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        }

        AnimatorState idleState = sm.AddState("Idle");
        idleState.motion = idle;

        AnimatorState walkState = sm.AddState("Walk");
        walkState.motion = walk;

        sm.defaultState = idleState;

        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.08f;
        idleToWalk.AddCondition(AnimatorConditionMode.Greater, 0.05f, "Speed");

        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.08f;
        walkToIdle.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed");

        EditorUtility.SetDirty(controller);
        return controller;
    }
}
