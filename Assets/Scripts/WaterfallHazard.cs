using UnityEngine;

public class WaterfallHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    void TryKillPlayer(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Purly_Health health = other.GetComponentInParent<Purly_Health>();
        if (health != null && !health.isDead)
        {
            health.Die();
        }
    }
}
