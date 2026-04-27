using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int food;
    public float timePlayed;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private const string SAVE_KEY = "HIGH_SCORES_TOP_5";

    public LeaderboardData data = new LeaderboardData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(string playerName, int score, int food, float timePlayed)
    {
        LeaderboardEntry entry = new LeaderboardEntry
        {
            playerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName,
            score = score,
            food = food,
            timePlayed = timePlayed
        };

        data.entries.Add(entry);

        data.entries.Sort((a, b) => b.score.CompareTo(a.score));

        if (data.entries.Count > 5)
            data.entries.RemoveRange(5, data.entries.Count - 5);

        SaveScores();
    }

    public List<LeaderboardEntry> GetTopScores()
    {
        return data.entries;
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        data = JsonUtility.FromJson<LeaderboardData>(json);
    }
	
    /*public void ClearScores()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        data.entries.Clear();
    }*/
}