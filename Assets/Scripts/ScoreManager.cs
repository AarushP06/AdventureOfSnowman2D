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

        score += 1;
        UpdateScoreText();
    }

    public void StopTrackingAndSave()
    {
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
            scoreText.text = "Score: " + score;
        }
    }

    void SaveScore()
    {
        ScoreStorage.SaveScore(playerName, score);
    }
}
