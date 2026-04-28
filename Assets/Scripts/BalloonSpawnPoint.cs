using UnityEngine;

public class BalloonSpawnPoint : MonoBehaviour
{
    // Spawn tiers let the spawner mix easy walk pickups with jump-based pickups.
    public enum SpawnTier
    {
        Easy,
        Medium,
        Hard
    }

    public SpawnTier tier = SpawnTier.Easy;
}
