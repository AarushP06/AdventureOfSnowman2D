using UnityEngine;

// Central SFX hub for gameplay.
// Other scripts call these methods instead of each owning their own AudioSource.
public class GameplayAudio : MonoBehaviour
{
    public static GameplayAudio Instance { get; private set; }

    public AudioClip jumpClip;
    public AudioClip landingSplashClip;
    public AudioClip yellowCollectClip;
    public AudioClip blackCollectClip;
    public AudioClip deathClip;
    public AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }

    public void PlayJump()
    {
        PlayClip(jumpClip);
    }

    public void PlayLandingSplash()
    {
        PlayClip(landingSplashClip);
    }

    public void PlayYellowCollect()
    {
        PlayClip(yellowCollectClip);
    }

    public void PlayBlackCollect()
    {
        PlayClip(blackCollectClip);
    }

    public void PlayDeath()
    {
        PlayClip(deathClip);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}
