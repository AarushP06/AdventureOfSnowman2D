using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SnowGun : MonoBehaviour
{
    public GameObject SnowBall;
    public Transform firePoint;
    public Transform purlyTarget;
    public float shootInterval = 10f;

    private const float InitialShotDelay = 2f;
    private const float SpawnOffsetDistance = 1f;

    private static readonly List<SnowGun> ActiveSnowGuns = new();
    private static int currentGunIndex;
    private static bool alternatingModeInitialized;
    private static float nextShotTime;

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

            if (currentGunIndex >= ActiveSnowGuns.Count)
            {
                currentGunIndex = 0;
            }
        }

        if (ActiveSnowGuns.Count == 0)
        {
            alternatingModeInitialized = false;
            nextShotTime = 0f;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        CacheUiGunRect();

        if (ActiveSnowGuns.Count <= 1)
        {
            InvokeRepeating(nameof(Shoot), InitialShotDelay, shootInterval);
            return;
        }

        if (!alternatingModeInitialized)
        {
            alternatingModeInitialized = true;
            currentGunIndex = 0;
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

        SnowGun activeGun = ActiveSnowGuns[currentGunIndex];
        activeGun.Shoot();

        nextShotTime = Time.time + activeGun.shootInterval;
        currentGunIndex = (currentGunIndex + 1) % ActiveSnowGuns.Count;
    }

    bool IsCoordinator()
    {
        return ActiveSnowGuns.Count > 1 && ReferenceEquals(ActiveSnowGuns[0], this);
    }

    void Shoot()
    {
        if (SnowBall == null || firePoint == null || purlyTarget == null) return;

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

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, uiGunRect.position);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPoint.x, screenPoint.y, Mathf.Abs(mainCamera.transform.position.z))
        );
        worldPoint.z = firePoint.position.z;
        return worldPoint;
    }

    Vector2 GetShotDirection(Vector3 shotOrigin)
    {
        Vector2 directionToPurly = (Vector2)(purlyTarget.position - shotOrigin);

        if (directionToPurly == Vector2.zero)
        {
            return Vector2.right;
        }

        return directionToPurly.normalized;
    }
}
