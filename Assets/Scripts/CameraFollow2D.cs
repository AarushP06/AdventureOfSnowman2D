using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.2f;
    public float fixedY = 0.25f;
    public float minX = -18f;
    public float maxX = 24f;
    public bool autoCalculateBounds = true;
    public Transform boundsRoot;
    public float boundsPadding = 0.25f;

    private float currentVelocityX;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        RefreshBounds();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        RefreshBounds();
    }
#endif

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Follow Purly only on X so the level reads like a side-scrolling platformer.
        float targetX = Mathf.Clamp(target.position.x, minX, maxX);
        float nextX = Mathf.SmoothDamp(transform.position.x, targetX, ref currentVelocityX, smoothTime);
        transform.position = new Vector3(nextX, fixedY, transform.position.z);
    }

    public void RefreshBounds()
    {
        if (!autoCalculateBounds)
        {
            return;
        }

        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        // Build camera clamps from the full visible level width instead of hard-coding them by hand.
        Transform root = boundsRoot != null ? boundsRoot : transform.parent;
        if (root == null || cam == null || !cam.orthographic)
        {
            return;
        }

        if (!TryGetHorizontalBounds(root, out float left, out float right))
        {
            return;
        }

        float halfWidth = cam.orthographicSize * cam.aspect;
        minX = left + halfWidth - boundsPadding;
        maxX = right - halfWidth + boundsPadding;

        if (minX > maxX)
        {
            float center = (left + right) * 0.5f;
            minX = center;
            maxX = center;
        }
    }

    bool TryGetHorizontalBounds(Transform root, out float left, out float right)
    {
        bool found = false;
        left = float.PositiveInfinity;
        right = float.NegativeInfinity;

        // Use both renderers and colliders so the camera respects the actual playable level width.
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (!renderer.enabled)
            {
                continue;
            }

            Bounds bounds = renderer.bounds;
            left = Mathf.Min(left, bounds.min.x);
            right = Mathf.Max(right, bounds.max.x);
            found = true;
        }

        foreach (Collider2D collider in root.GetComponentsInChildren<Collider2D>(true))
        {
            if (!collider.enabled)
            {
                continue;
            }

            Bounds bounds = collider.bounds;
            left = Mathf.Min(left, bounds.min.x);
            right = Mathf.Max(right, bounds.max.x);
            found = true;
        }

        return found;
    }
}
