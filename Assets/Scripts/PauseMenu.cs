using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string landingScenePath = "Assets/Scenes/SampleScene.unity";

    private bool isPaused;

    void Awake()
    {
        // Always resume normal time when this scene loads.
        Time.timeScale = 1f;
        SetPauseState(false);
    }

    void OnDestroy()
    {
        // Safety reset so time does not stay frozen if the object is destroyed while paused.
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Escape toggles the pause panel during gameplay.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPauseState(!isPaused);
    }

    public void ResumeGame()
    {
        SetPauseState(false);
    }

    public void RestartGame()
    {
        // Reload the active gameplay scene from the beginning.
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMenu()
    {
        // Return to the landing/menu scene configured in the Inspector.
        Time.timeScale = 1f;
        SceneManager.LoadScene(landingScenePath);
    }

    void SetPauseState(bool shouldPause)
    {
        isPaused = shouldPause;
        // Time scale 0 freezes gameplay while keeping the UI responsive.
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }
}
