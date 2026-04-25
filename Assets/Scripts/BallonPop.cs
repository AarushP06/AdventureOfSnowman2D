using System.Collections;
using UnityEngine;

public class BalloonPop : MonoBehaviour
{
    public bool respawnAtStartPosition = true;

    private enum WallSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    private const float RespawnDelay = 2f;
    private const float MinHorizontalX = -6f;
    private const float MaxHorizontalX = 6.8f;
    private const float MinVerticalY = -3.2f;
    private const float MaxVerticalY = 3.2f;
    private const float TopWallY = 3.57f;
    private const float BottomWallY = -3.44f;
    private const float LeftWallX = -6.3f;
    private const float RightWallX = 7.05f;

    private Collider2D balloonCollider;
    private SpriteRenderer balloonRenderer;
    private bool isRespawning;
    private WallSide wallSide;
    private Vector3 startPosition;

    void Awake()
    {
        balloonCollider = GetComponent<Collider2D>();
        balloonRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        CacheWallSide();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isRespawning || !other.CompareTag("Player"))
        {
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoint();
        }

        StartCoroutine(RespawnOnSameWall());
    }

    void CacheWallSide()
    {
        Vector2 startPosition = transform.position;

        if (Mathf.Abs(startPosition.x) > Mathf.Abs(startPosition.y))
        {
            wallSide = startPosition.x < 0f ? WallSide.Left : WallSide.Right;
            return;
        }

        wallSide = startPosition.y < 0f ? WallSide.Bottom : WallSide.Top;
    }

    IEnumerator RespawnOnSameWall()
    {
        isRespawning = true;
        balloonCollider.enabled = false;

        if (balloonRenderer != null)
        {
            balloonRenderer.enabled = false;
        }

        yield return new WaitForSeconds(RespawnDelay);

        transform.position = respawnAtStartPosition ? startPosition : GetRandomPositionOnWall();

        if (balloonRenderer != null)
        {
            balloonRenderer.enabled = true;
        }

        balloonCollider.enabled = true;
        isRespawning = false;
    }

    Vector2 GetRandomPositionOnWall()
    {
        switch (wallSide)
        {
            case WallSide.Top:
                return new Vector2(Random.Range(MinHorizontalX, MaxHorizontalX), TopWallY);
            case WallSide.Bottom:
                return new Vector2(Random.Range(MinHorizontalX, MaxHorizontalX), BottomWallY);
            case WallSide.Left:
                return new Vector2(LeftWallX, Random.Range(MinVerticalY, MaxVerticalY));
            default:
                return new Vector2(RightWallX, Random.Range(MinVerticalY, MaxVerticalY));
        }
    }
}
