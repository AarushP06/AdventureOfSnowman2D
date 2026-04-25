using UnityEngine;

public class WaterfallHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Purly_Health health = other.GetComponent<Purly_Health>();
        if (health != null)
        {
            health.Die();
        }
    }
}
