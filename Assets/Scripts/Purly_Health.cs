using UnityEngine;

public class Purly_Health : MonoBehaviour
{
    public bool isDead = false;

    public void Die()
    {
        Debug.Log("Purly Die() was called");

        if (isDead) return;

        isDead = true;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopTrackingAndSave();
        }

        Destroy(gameObject);
    }
}
