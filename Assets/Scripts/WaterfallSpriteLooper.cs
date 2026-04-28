using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterfallSpriteLooper : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 8f;
    public int startFrameOffset;

    private SpriteRenderer spriteRenderer;
    private int currentFrame = -1;
    private double lastEditorTime;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentFrame = -1;

#if UNITY_EDITOR
        // Editor update lets the waterfall animate in the scene view, not just in play mode.
        lastEditorTime = EditorApplication.timeSinceStartup;
        EditorApplication.update -= EditorTick;
        EditorApplication.update += EditorTick;
#endif

        RefreshFrame(true);
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorTick;
#endif
    }

    void Update()
    {
        // In play mode, advance the waterfall frames using unscaled time for a steady loop.
        if (Application.isPlaying)
        {
            RefreshFrame(false);
        }
    }

#if UNITY_EDITOR
    void EditorTick()
    {
        if (this == null || Application.isPlaying)
        {
            return;
        }

        // Throttle editor refreshes so scene-view animation is smooth without updating every tiny tick.
        double now = EditorApplication.timeSinceStartup;
        if (now - lastEditorTime < 0.02d)
        {
            return;
        }

        lastEditorTime = now;
        RefreshFrame(false);
    }
#endif

    void RefreshFrame(bool force)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null || frames == null || frames.Length == 0 || framesPerSecond <= 0f)
        {
            return;
        }

        // Calculate which sprite frame should be visible at the current time.
        double time = Application.isPlaying
            ? Time.unscaledTimeAsDouble
#if UNITY_EDITOR
            : EditorApplication.timeSinceStartup
#else
            : Time.realtimeSinceStartupAsDouble
#endif
            ;

        int frameIndex = (int)(time * framesPerSecond);
        frameIndex = (frameIndex + startFrameOffset) % frames.Length;

        if (!force && frameIndex == currentFrame)
        {
            return;
        }

        currentFrame = frameIndex;
        spriteRenderer.sprite = frames[frameIndex];

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Mark the renderer dirty so the editor redraws the new frame immediately.
            EditorUtility.SetDirty(spriteRenderer);
        }
#endif
    }
}
