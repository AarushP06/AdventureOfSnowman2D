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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoint();
        }

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
