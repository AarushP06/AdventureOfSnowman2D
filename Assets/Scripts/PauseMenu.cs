using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string landingScenePath = "Assets/Scenes/SampleScene.unity";

    private bool isPaused;

    void Awake()
    {
        Time.timeScale = 1f;
        SetPauseState(false);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(landingScenePath);
    }

    void SetPauseState(bool shouldPause)
    {
        isPaused = shouldPause;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }
}
