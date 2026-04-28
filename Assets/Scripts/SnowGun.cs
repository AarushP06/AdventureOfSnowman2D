using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SnowGun : MonoBehaviour
{
    public GameObject SnowBall;
    public Transform firePoint;
    public Transform purlyTarget;
    public float shootInterval = 10f;

    // These defaults keep the attack feeling varied without making it too unfair.
    private const float InitialShotDelay = 2f;
    private const float SpawnOffsetDistance = 1f;
    private const float DefaultMinInterval = 7f;
    private const float DefaultMaxInterval = 13f;
    private const float DefaultAimOffsetX = 0.9f;
    private const float DefaultAimOffsetY = 1.2f;
    private const float DefaultAngleJitter = 10f;

    private static readonly List<SnowGun> ActiveSnowGuns = new();
    private static bool alternatingModeInitialized;
    private static float nextShotTime;
    private static SnowGun lastFiredGun;

    private Camera mainCamera;
    private RectTransform uiGunRect;

    void OnEnable()
    {
        if (!ActiveSnowGuns.Contains(this))
        {
            ActiveSnowGuns.Add(this);
        }
    }

    void OnDisable()
    {
        CancelInvoke();

        int removedIndex = ActiveSnowGuns.IndexOf(this);
        if (removedIndex >= 0)
        {
            ActiveSnowGuns.RemoveAt(removedIndex);
        }

        if (ActiveSnowGuns.Count == 0)
        {
            alternatingModeInitialized = false;
            nextShotTime = 0f;
            lastFiredGun = null;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        CacheUiGunRect();

        // A single active gun can schedule itself directly.
        if (ActiveSnowGuns.Count <= 1)
        {
            alternatingModeInitialized = true;
            nextShotTime = Time.time + InitialShotDelay;
            return;
        }

        if (!alternatingModeInitialized)
        {
            // With multiple guns, only one shared coordinator drives the firing order.
            alternatingModeInitialized = true;
            nextShotTime = Time.time + InitialShotDelay;
        }
    }

    void Update()
    {
        if (!alternatingModeInitialized || !IsCoordinator())
        {
            return;
        }

        if (Time.time < nextShotTime || ActiveSnowGuns.Count == 0)
        {
            return;
        }

        SnowGun activeGun = SelectNextGun();

        if (activeGun == null)
        {
            return;
        }

        // Fire one gun, remember it, then wait a random amount of time before the next shot.
        activeGun.Shoot();
        lastFiredGun = activeGun;
        nextShotTime = Time.time + activeGun.GetNextShotDelay();
    }

    bool IsCoordinator()
    {
        return ActiveSnowGuns.Count > 0 && ReferenceEquals(ActiveSnowGuns[0], this);
    }

    void Shoot()
    {
        if (SnowBall == null || firePoint == null || purlyTarget == null) return;

        // Spawn from the matching UI gun position so the shots appear to come from the HUD corners.
        Vector3 origin = GetShotOrigin();
        Vector2 shotDirection = GetShotDirection(origin);
        Vector3 spawnPosition = origin + (Vector3)(shotDirection * SpawnOffsetDistance);
        GameObject newSnowball = Instantiate(SnowBall, spawnPosition, Quaternion.identity);

        SnowBall snowballScript = newSnowball.GetComponent<SnowBall>();
        Collider2D gunCollider = GetComponent<Collider2D>();

        if (snowballScript != null)
        {
            snowballScript.IgnoreCollisionWith(gunCollider);
            snowballScript.SetMoveDirection(shotDirection);
        }
    }

    void CacheUiGunRect()
    {
        // Left/right world guns map to left/right UI anchors by their scene position.
        string uiGunName = transform.position.x < 0f ? "SnowGunUI_Left" : "SnowGunUI_Right";
        GameObject uiGun = GameObject.Find(uiGunName);
        uiGunRect = uiGun != null ? uiGun.GetComponent<RectTransform>() : null;
    }

    Vector3 GetShotOrigin()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (uiGunRect == null)
        {
            CacheUiGunRect();
        }

        if (mainCamera == null || uiGunRect == null)
        {
            return firePoint.position;
        }

        // Convert the UI image position into a world-space spawn point for the projectile.
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, uiGunRect.position);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPoint.x, screenPoint.y, Mathf.Abs(mainCamera.transform.position.z))
        );
        worldPoint.z = firePoint.position.z;
        return worldPoint;
    }

    Vector2 GetShotDirection(Vector3 shotOrigin)
    {
        // Aim near Purly instead of exactly at the center every time.
        Vector2 targetPoint = (Vector2)purlyTarget.position + GetRandomAimOffset();
        Vector2 directionToPurly = targetPoint - (Vector2)shotOrigin;

        if (directionToPurly == Vector2.zero)
        {
            return Vector2.right;
        }

        return ApplyAngleJitter(directionToPurly.normalized);
    }

    SnowGun SelectNextGun()
    {
        if (ActiveSnowGuns.Count == 0)
        {
            return null;
        }

        if (ActiveSnowGuns.Count == 1)
        {
            return ActiveSnowGuns[0];
        }

        List<SnowGun> candidates = new();

        // Pick randomly from all active guns except the one that just fired.
        for (int i = 0; i < ActiveSnowGuns.Count; i++)
        {
            SnowGun gun = ActiveSnowGuns[i];

            if (gun != null && gun != lastFiredGun)
            {
                candidates.Add(gun);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    float GetNextShotDelay()
    {
        // Keep each gun's configured interval as the baseline, then randomize around it.
        float baseline = shootInterval > 0f ? shootInterval : 10f;
        float minDelay = Mathf.Min(GetConfiguredMinInterval(baseline), GetConfiguredMaxInterval(baseline));
        float maxDelay = Mathf.Max(GetConfiguredMinInterval(baseline), GetConfiguredMaxInterval(baseline));
        return Random.Range(minDelay, maxDelay);
    }

    float GetConfiguredMinInterval(float baseline)
    {
        return Mathf.Max(0.5f, Mathf.Max(baseline - 3f, DefaultMinInterval));
    }

    float GetConfiguredMaxInterval(float baseline)
    {
        return Mathf.Max(GetConfiguredMinInterval(baseline) + 0.25f, Mathf.Max(baseline + 3f, DefaultMaxInterval));
    }

    Vector2 GetRandomAimOffset()
    {
        // Small offset makes some snowballs slightly high/low or left/right of Purly.
        return new Vector2(
            Random.Range(-DefaultAimOffsetX, DefaultAimOffsetX),
            Random.Range(-DefaultAimOffsetY, DefaultAimOffsetY)
        );
    }

    Vector2 ApplyAngleJitter(Vector2 direction)
    {
        // Extra angle jitter stops repeated shots from tracing the exact same line.
        float angleOffset = Random.Range(-DefaultAngleJitter, DefaultAngleJitter);
        return Quaternion.Euler(0f, 0f, angleOffset) * direction;
    }
}
