using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BalloonPop : MonoBehaviour
{
    private Collider2D balloonCollider;
    private SpriteRenderer balloonRenderer;
    private PlatformBalloonSpawner spawner;

    void Awake()
    {
        balloonCollider = GetComponent<Collider2D>();
        balloonRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Every balloon is worth exactly one point for Assignment 04.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoint();
        }

        // If a spawner owns this balloon, let it handle the next random respawn.
        if (spawner != null)
        {
            spawner.HandleCollected(this);
        }
        else
        {
            Hide();
        }
    }

    public void AssignSpawner(PlatformBalloonSpawner balloonSpawner)
    {
        spawner = balloonSpawner;
    }

    public void ShowAt(Vector3 position)
    {
        // Reuse the same balloon object instead of instantiating and destroying repeatedly.
        transform.position = position;

        if (balloonRenderer != null)
        {
            balloonRenderer.enabled = true;
        }

        if (balloonCollider != null)
        {
            balloonCollider.enabled = true;
        }
    }

    public void Hide()
    {
        // Disabling both visuals and collider makes the balloon fully inactive until respawn.
        if (balloonRenderer != null)
        {
            balloonRenderer.enabled = false;
        }

        if (balloonCollider != null)
        {
            balloonCollider.enabled = false;
        }
    }
}
