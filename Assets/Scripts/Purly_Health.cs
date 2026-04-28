using UnityEngine;

public class Purly_Health : MonoBehaviour
{
    public bool isDead = false;

    public void Die()
    {
        // Prevent duplicate death handling from overlapping hazards or projectiles.
        if (isDead) return;

        isDead = true;

        if (ScoreManager.Instance != null)
        {
            // Stop score changes and write the final score before removing Purly from the scene.
            ScoreManager.Instance.StopTrackingAndSave();
        }

        Destroy(gameObject);
    }
}
