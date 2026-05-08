using UnityEngine;

// Handles one collectible balloon in the level.
// The spawner decides where it appears; this script only handles score, SFX, and hide/show.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BalloonPop : MonoBehaviour
{
    public enum BalloonType
    {
        Yellow,
        Black
    }

    [SerializeField] private BalloonType balloonType = BalloonType.Yellow;

    private Collider2D balloonCollider;
    private SpriteRenderer balloonRenderer;
    private PlatformBalloonSpawner spawner;

    public BalloonType Type => balloonType;
    public bool IsBlackBalloon => balloonType == BalloonType.Black;
    public bool IsYellowBalloon => balloonType == BalloonType.Yellow;
    public int ScoreDelta => IsBlackBalloon ? -3 : 1;

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

        // Yellow balloons add one point; black balloons deduct three points.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(ScoreDelta);
        }

        if (GameplayAudio.Instance != null)
        {
            if (IsBlackBalloon)
            {
                GameplayAudio.Instance.PlayBlackCollect();
            }
            else
            {
                GameplayAudio.Instance.PlayYellowCollect();
            }
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
