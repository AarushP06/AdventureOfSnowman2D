using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.2f;
    public float fixedY = 0.25f;
    public float minX = -18f;
    public float maxX = 24f;

    private float currentVelocityX;

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float targetX = Mathf.Clamp(target.position.x, minX, maxX);
        float nextX = Mathf.SmoothDamp(transform.position.x, targetX, ref currentVelocityX, smoothTime);
        transform.position = new Vector3(nextX, fixedY, transform.position.z);
    }
}
