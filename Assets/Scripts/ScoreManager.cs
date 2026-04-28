using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public Text scoreText;

    private int score;
    private bool isTracking = true;
    private string playerName;

    public bool IsTracking => isTracking;

    void Awake()
    {
        // Basic singleton pattern so balloons and death hazards can reach the score system easily.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        playerName = ScoreStorage.GetCurrentPlayerName();
        UpdateScoreText();
    }

    public void AddPoint()
    {
        if (!isTracking)
        {
            return;
        }

        // Assignment 04 uses one point per balloon.
        score += 1;
        UpdateScoreText();
    }

    public void StopTrackingAndSave()
    {
        // Save only once when the run ends.
        if (!isTracking)
        {
            return;
        }

        isTracking = false;
        SaveScore();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            // Keep the HUD text in sync with the current score value.
            scoreText.text = "Score: " + score;
        }
    }

    void SaveScore()
    {
        ScoreStorage.SaveScore(playerName, score);
    }
}
