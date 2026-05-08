using UnityEngine;

// Applies simple Unity audio effects to gameplay music.
// Right now pause muffles the music by closing a low-pass filter, then restores it on resume.
public class GameplayAudioEffects : MonoBehaviour
{
    public AudioSource gameplayMusicSource;
    public AudioLowPassFilter gameplayMusicLowPass;
    public float normalCutoffFrequency = 22000f;
    public float pausedCutoffFrequency = 900f;
    public float transitionSpeed = 8f;

    void Awake()
    {
        if (gameplayMusicSource == null)
        {
            gameplayMusicSource = GetComponent<AudioSource>();
        }

        if (gameplayMusicLowPass == null)
        {
            gameplayMusicLowPass = GetComponent<AudioLowPassFilter>();
        }

        if (gameplayMusicLowPass != null)
        {
            gameplayMusicLowPass.cutoffFrequency = normalCutoffFrequency;
        }
    }

    void Update()
    {
        if (gameplayMusicLowPass == null)
        {
            return;
        }

        float targetCutoff = Time.timeScale <= 0.01f ? pausedCutoffFrequency : normalCutoffFrequency;
        gameplayMusicLowPass.cutoffFrequency = Mathf.Lerp(
            gameplayMusicLowPass.cutoffFrequency,
            targetCutoff,
            Time.unscaledDeltaTime * transitionSpeed
        );
    }
}
