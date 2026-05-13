using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class CreateBottomBigEarsAnimator
{
    private const string BasePath = "Assets/Art/GeneratedCharacters/single_sources/bottom_big_ears";
    private const string IdlePath = BasePath + "/idle.png";
    private const string Walk1Path = BasePath + "/walk_1.png";
    private const string Walk2Path = BasePath + "/walk_2.png";

    private const string OutputDir = BasePath + "/Animator";
    private const string IdleClipPath = OutputDir + "/bottom_big_ears_idle.anim";
    private const string WalkClipPath = OutputDir + "/bottom_big_ears_walk.anim";
    private const string ControllerPath = OutputDir + "/bottom_big_ears.controller";

    [MenuItem("Tools/Animation/Create Bottom Big Ears Animator")]
    public static void CreateAnimatorAssets()
    {
        EnsureTextureSettings(IdlePath);
        EnsureTextureSettings(Walk1Path);
        EnsureTextureSettings(Walk2Path);

        Sprite idle = AssetDatabase.LoadAssetAtPath<Sprite>(IdlePath);
        Sprite walk1 = AssetDatabase.LoadAssetAtPath<Sprite>(Walk1Path);
        Sprite walk2 = AssetDatabase.LoadAssetAtPath<Sprite>(Walk2Path);

        if (idle == null || walk1 == null || walk2 == null)
        {
            Debug.LogError("Sprite load failed. Check idle.png, walk_1.png, walk_2.png in bottom_big_ears folder.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(OutputDir))
        {
            AssetDatabase.CreateFolder(BasePath, "Animator");
        }

        AnimationClip idleClip = CreateIdleClip(idle, IdleClipPath);
        AnimationClip walkClip = CreateWalkClip(walk1, walk2, WalkClipPath);
        AnimatorController controller = CreateController(idleClip, walkClip, ControllerPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Animator assets: {controller.name}");
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

        if (controller.parameters.Length == 0)
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
