using UnityEngine;

// Single source of truth for player death.
// All hazards call Die(), which keeps the score-save and death SFX flow consistent.
public class Purly_Health : MonoBehaviour
{
    public bool isDead = false;

    public void Die()
    {
        // Prevent duplicate death handling from overlapping hazards or projectiles.
        if (isDead) return;

        isDead = true;

        if (GameplayAudio.Instance != null)
        {
            GameplayAudio.Instance.PlayDeath();
        }

        if (ScoreManager.Instance != null)
        {
            // Stop score changes and write the final score before removing Purly from the scene.
            ScoreManager.Instance.StopTrackingAndSave();
        }

        Destroy(gameObject);
    }
}
