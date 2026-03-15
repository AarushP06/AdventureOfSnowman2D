using UnityEngine;

public class Purly_Health : MonoBehaviour
{
    public bool isDead = false;

    public void Die()
    {
        Debug.Log("Purly Die() was called");

        if (isDead) return;

        isDead = true;
        Destroy(gameObject);
    }
}
