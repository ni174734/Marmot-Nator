using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text foodText;
    public TMP_Text timeText;
    public TMP_InputField nameInput;

    [Header("Scenes")]
    public string highScoreSceneName = "HighScores";
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + RunStats.finalScore;

        if (foodText != null)
            foodText.text = "Food Ate: " + RunStats.finalFood;

        if (timeText != null)
            timeText.text = "Time: " + FormatTime(RunStats.finalTimePlayed);
    }

    public void SubmitScore()
    {
        string playerName = nameInput != null ? nameInput.text : "Player";

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddScore(
                playerName,
                RunStats.finalScore,
                RunStats.finalFood,
                RunStats.finalTimePlayed
            );
        }

        //SceneManager.LoadScene(highScoreSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return minutes + ":" + secs.ToString("00");
    }
}