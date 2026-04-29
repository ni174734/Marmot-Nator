using UnityEngine;
using TMPro;

public class HighScoreUI : MonoBehaviour
{
    [Header("Name Column")]
    public TMP_Text[] nameTexts;

    [Header("Score Column")]
    public TMP_Text[] scoreTexts;

    [Header("Food Column")]
    public TMP_Text[] foodTexts;

    [Header("Playtime Column")]
    public TMP_Text[] timeTexts;

    private void Start()
    {
        RefreshLeaderboard();
		if (InputModeManager.Instance != null) InputModeManager.Instance.SwitchToUI();
    }

    public void RefreshLeaderboard()
    {
        if (LeaderboardManager.Instance == null)
        {
            Debug.LogError("LeaderboardManager not found.");
            return;
        }

        var entries = LeaderboardManager.Instance.GetTopScores();

        for (int i = 0; i < 5; i++)
        {
            if (i < entries.Count)
            {
                LeaderboardEntry e = entries[i];

                nameTexts[i].text = $"{i + 1}. {e.playerName}";
                scoreTexts[i].text = e.score.ToString("00000");
                foodTexts[i].text = e.food.ToString("000");
                timeTexts[i].text = FormatTime(e.timePlayed);
            }
            else
            {
                nameTexts[i].text = $"{i + 1}.";
                scoreTexts[i].text = "00000";
                foodTexts[i].text = "000";
                timeTexts[i].text = "00:00";
            }
        }
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);

        return minutes.ToString("00") + ":" + secs.ToString("00");
    }
}