using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class ScoreStorage
{
    [Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
        public string savedAt;
    }

    [Serializable]
    private class ScoreHistoryData
    {
        public List<ScoreEntry> entries = new();
    }

    private const string CurrentPlayerNameKey = "CurrentPlayerName";
    private const string LastSavedPlayerNameKey = "LastSavedPlayerName";
    private const string ScoreHistoryFileName = "score_history.json";

    public static string GetCurrentPlayerName()
    {
        return PlayerPrefs.GetString(CurrentPlayerNameKey, "Player");
    }

    public static void SetCurrentPlayerName(string playerName)
    {
        // Keep player names trimmed and non-empty before saving them into PlayerPrefs.
        string safeName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
        PlayerPrefs.SetString(CurrentPlayerNameKey, safeName);
        PlayerPrefs.Save();
    }

    public static bool HasSavedPlayer()
    {
        return PlayerPrefs.HasKey(LastSavedPlayerNameKey);
    }

    public static string GetLastSavedPlayerName()
    {
        return PlayerPrefs.GetString(LastSavedPlayerNameKey, "Player");
    }

    public static void SaveScore(string playerName, int score)
    {
        string safeName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();

        // Save both the latest run and a short high-score history.
        ScoreEntry latestEntry = new ScoreEntry
        {
            playerName = safeName,
            score = score,
            savedAt = DateTime.UtcNow.ToString("o")
        };

        string latestScorePath = Path.Combine(Application.persistentDataPath, "player_score.json");
        File.WriteAllText(latestScorePath, JsonUtility.ToJson(latestEntry, true));

        ScoreHistoryData history = LoadHistoryData();
        history.entries.Add(latestEntry);
        // Keep only the top five entries so the menu list stays short and readable.
        history.entries = history.entries
            .OrderByDescending(entry => entry.score)
            .ThenByDescending(entry => entry.savedAt)
            .Take(5)
            .ToList();

        string historyPath = Path.Combine(Application.persistentDataPath, ScoreHistoryFileName);
        File.WriteAllText(historyPath, JsonUtility.ToJson(history, true));

        PlayerPrefs.SetString(LastSavedPlayerNameKey, safeName);
        PlayerPrefs.Save();
    }

    public static List<ScoreEntry> LoadTopScores()
    {
        return LoadHistoryData().entries
            .OrderByDescending(entry => entry.score)
            .ThenByDescending(entry => entry.savedAt)
            .Take(5)
            .ToList();
    }

    private static ScoreHistoryData LoadHistoryData()
    {
        string historyPath = Path.Combine(Application.persistentDataPath, ScoreHistoryFileName);
        if (!File.Exists(historyPath))
        {
            // Start from an empty history when the score file does not exist yet.
            return new ScoreHistoryData();
        }

        string json = File.ReadAllText(historyPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new ScoreHistoryData();
        }

        // Gracefully recover even if the json file somehow deserializes to null.
        ScoreHistoryData history = JsonUtility.FromJson<ScoreHistoryData>(json);
        return history ?? new ScoreHistoryData();
    }
}
