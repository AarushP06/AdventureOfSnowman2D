using UnityEngine;

// Owns the landing splash particle system and repositions it to Purly's feet when a landing happens.
public class LandingSplashParticles : MonoBehaviour
{
    public static LandingSplashParticles Instance { get; private set; }

    public ParticleSystem splashParticleSystem;
    public float verticalOffset = 0.05f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (splashParticleSystem == null)
        {
            splashParticleSystem = GetComponent<ParticleSystem>();
        }
    }

    public void PlayAt(Vector3 worldPosition)
    {
        if (splashParticleSystem == null)
        {
            return;
        }

        transform.position = new Vector3(worldPosition.x, worldPosition.y + verticalOffset, worldPosition.z);
        splashParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        splashParticleSystem.Play(true);
    }
}
