using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformBalloonSpawner : MonoBehaviour
{
    public Transform spawnRoot;
    public Transform balloonRoot;
    public float respawnDelay = 1.5f;

    private readonly Dictionary<BalloonPop, BalloonSpawnPoint.SpawnTier> balloonTiers = new();
    private readonly Dictionary<BalloonPop, BalloonSpawnPoint> activePoints = new();
    private readonly List<BalloonSpawnPoint> spawnPoints = new();

    void Awake()
    {
        // Cache once at startup so the spawner knows every valid point and every reusable balloon.
        CacheSpawnPoints();
        CacheBalloons();
    }

    void Start()
    {
        SpawnAllBalloons();
    }

    public void HandleCollected(BalloonPop balloon)
    {
        if (balloon == null)
        {
            return;
        }

        // Hide immediately, then respawn later at another valid point.
        balloon.Hide();
        activePoints.Remove(balloon);
        StartCoroutine(RespawnBalloon(balloon));
    }

    IEnumerator RespawnBalloon(BalloonPop balloon)
    {
        yield return new WaitForSeconds(respawnDelay);

        if (balloon == null)
        {
            yield break;
        }

        PlaceBalloon(balloon, GetTierFor(balloon));
    }

    void CacheSpawnPoints()
    {
        spawnPoints.Clear();

        // Spawn points are placed by hand so random balloons stay reachable and never appear in hazards.
        Transform root = spawnRoot != null ? spawnRoot : transform;
        spawnPoints.AddRange(root.GetComponentsInChildren<BalloonSpawnPoint>(true));
    }

    void CacheBalloons()
    {
        balloonTiers.Clear();
        activePoints.Clear();

        Transform root = balloonRoot != null ? balloonRoot : transform;
        BalloonPop[] balloons = root.GetComponentsInChildren<BalloonPop>(true);

        for (int i = 0; i < balloons.Length; i++)
        {
            BalloonPop balloon = balloons[i];
            if (balloon == null)
            {
                continue;
            }

            balloon.AssignSpawner(this);
            balloon.Hide();
            // Spread the starting balloon pool across easy, medium, and hard spawn tiers.
            balloonTiers[balloon] = GetTierForIndex(i);
        }
    }

    void SpawnAllBalloons()
    {
        foreach (BalloonPop balloon in balloonTiers.Keys.ToArray())
        {
            PlaceBalloon(balloon, balloonTiers[balloon]);
        }
    }

    void PlaceBalloon(BalloonPop balloon, BalloonSpawnPoint.SpawnTier tier)
    {
        BalloonSpawnPoint point = PickPoint(tier, balloon);
        if (point == null)
        {
            return;
        }

        // Track which point is occupied so two active balloons do not stack on the same spot.
        activePoints[balloon] = point;
        balloon.ShowAt(point.transform.position);
    }

    BalloonSpawnPoint PickPoint(BalloonSpawnPoint.SpawnTier tier, BalloonPop balloon)
    {
        // First prefer unused points in the correct difficulty tier.
        List<BalloonSpawnPoint> candidates = spawnPoints
            .Where(point => point != null && point.tier == tier)
            .Where(point => !activePoints.Where(pair => pair.Key != balloon).Any(pair => pair.Value == point))
            .ToList();

        if (candidates.Count == 0)
        {
            // If every point in that tier is occupied, fall back to any point in the same tier.
            candidates = spawnPoints
                .Where(point => point != null && point.tier == tier)
                .ToList();
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        BalloonSpawnPoint currentPoint = activePoints.TryGetValue(balloon, out BalloonSpawnPoint activePoint)
            ? activePoint
            : null;

        if (candidates.Count > 1 && currentPoint != null)
        {
            // Avoid respawning the same balloon at the exact same point when another option exists.
            candidates.Remove(currentPoint);
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    BalloonSpawnPoint.SpawnTier GetTierFor(BalloonPop balloon)
    {
        return balloonTiers.TryGetValue(balloon, out BalloonSpawnPoint.SpawnTier tier)
            ? tier
            : BalloonSpawnPoint.SpawnTier.Easy;
    }

    BalloonSpawnPoint.SpawnTier GetTierForIndex(int index)
    {
        // Rotate the initial balloon pool through easy, medium, and hard tiers.
        int mod = index % 3;
        if (mod == 1)
        {
            return BalloonSpawnPoint.SpawnTier.Medium;
        }

        if (mod == 2)
        {
            return BalloonSpawnPoint.SpawnTier.Hard;
        }

        return BalloonSpawnPoint.SpawnTier.Easy;
    }
}
