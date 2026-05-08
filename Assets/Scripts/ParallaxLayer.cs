using UnityEngine;

// Moves a background layer by a fraction of the camera movement to create parallax depth.
public class ParallaxLayer : MonoBehaviour
{
    public Transform cameraTarget;
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;
    public bool affectX = true;
    public bool affectY;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    void Start()
    {
        if (cameraTarget == null && Camera.main != null)
        {
            cameraTarget = Camera.main.transform;
        }

        startPosition = transform.position;
        cameraStartPosition = cameraTarget != null ? cameraTarget.position : Vector3.zero;
    }

    void LateUpdate()
    {
        if (cameraTarget == null)
        {
            return;
        }

        Vector3 cameraDelta = cameraTarget.position - cameraStartPosition;
        float nextX = affectX ? startPosition.x + (cameraDelta.x * parallaxFactor) : transform.position.x;
        float nextY = affectY ? startPosition.y + (cameraDelta.y * parallaxFactor) : startPosition.y;
        transform.position = new Vector3(nextX, nextY, startPosition.z);
    }
}
