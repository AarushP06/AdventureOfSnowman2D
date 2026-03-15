using UnityEngine;

public class SnowGun : MonoBehaviour
{
    public GameObject SnowBall;
    public Transform firePoint;
    public Transform purlyTarget;
    public float shootInterval = 10f;

    void Start()
    {
        InvokeRepeating(nameof(Shoot), 2f, shootInterval);
    }

    void Shoot()
    {
        if (SnowBall == null || firePoint == null || purlyTarget == null) return;

        GameObject newSnowball = Instantiate(SnowBall, firePoint.position, Quaternion.identity);

        SnowBall snowballScript = newSnowball.GetComponent<SnowBall>();

        if (snowballScript != null)
        {
            snowballScript.target = purlyTarget;
        }
    }
}