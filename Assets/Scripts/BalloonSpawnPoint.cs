using UnityEngine;

public class BalloonSpawnPoint : MonoBehaviour
{
    public enum SpawnTier
    {
        Easy,
        Medium,
        Hard
    }

    public SpawnTier tier = SpawnTier.Easy;
}
