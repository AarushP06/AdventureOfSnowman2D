using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string gameplayScenePath = "Assets/Adventure of Snowman 2D..unity";
    public InputField nameInputField;
    public Text highScoresText;
    public Button savedGameButton;
    public GameObject highScoresPanel;

    void Start()
    {
        if (nameInputField != null)
        {
            nameInputField.text = ScoreStorage.GetCurrentPlayerName();
        }

        if (highScoresPanel != null)
        {
            highScoresPanel.SetActive(false);
        }

        RefreshHighScores();
        RefreshSavedGameButton();
    }

    public void StartNewGame()
    {
        ScoreStorage.SetCurrentPlayerName(GetEnteredName());
        SceneManager.LoadScene(gameplayScenePath);
    }

    public void StartSavedGame()
    {
        string savedPlayerName = ScoreStorage.HasSavedPlayer()
            ? ScoreStorage.GetLastSavedPlayerName()
            : GetEnteredName();

        ScoreStorage.SetCurrentPlayerName(savedPlayerName);
        SceneManager.LoadScene(gameplayScenePath);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowHighScores()
    {
        RefreshHighScores();

        if (highScoresPanel != null)
        {
            highScoresPanel.SetActive(true);
        }
    }

    public void HideHighScores()
    {
        if (highScoresPanel != null)
        {
            highScoresPanel.SetActive(false);
        }
    }

    public void RefreshHighScores()
    {
        if (highScoresText == null)
        {
            return;
        }

        List<ScoreStorage.ScoreEntry> entries = ScoreStorage.LoadTopScores();
        if (entries.Count == 0)
        {
            highScoresText.text = "No scores yet";
            return;
        }

        List<string> lines = new();
        for (int i = 0; i < entries.Count; i++)
        {
            ScoreStorage.ScoreEntry entry = entries[i];
            lines.Add((i + 1) + ". " + entry.playerName + " - " + entry.score);
        }

        highScoresText.text = string.Join("\n", lines);
    }

    public void RefreshSavedGameButton()
    {
        if (savedGameButton != null)
        {
            savedGameButton.interactable = ScoreStorage.HasSavedPlayer();
        }
    }

    string GetEnteredName()
    {
        if (nameInputField == null || string.IsNullOrWhiteSpace(nameInputField.text))
        {
            return "Player";
        }

        return nameInputField.text.Trim();
    }
}
