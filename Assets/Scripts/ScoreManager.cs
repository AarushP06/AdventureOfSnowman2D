using UnityEngine;
using UnityEngine.UI;

// Owns the current run score and keeps the HUD text synchronized with it.
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
        AddScore(1);
    }

    public void AddScore(int amount)
    {
        if (!isTracking)
        {
            return;
        }

        // Assignment 05 keeps the same score HUD but allows penalty balloons too.
        score += amount;
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
