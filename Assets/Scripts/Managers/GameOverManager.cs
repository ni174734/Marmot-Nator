using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Scene Settings")]
    public string gameOverSceneName = "GameOver";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER: ");

        // simplest: reload scene after a delay
        // Invoke(nameof(Reload), 1.5f);
		
		if (ScoreManager.Instance != null)
        {
            RunStats.finalScore = ScoreManager.Instance.score;
            RunStats.finalFood = ScoreManager.Instance.totalFood;
        }

        if (GameManager.Instance != null)
        {
            RunStats.finalTimePlayed = GameManager.Instance.GetTotalTimePlayed();
        }
		
        Time.timeScale = 1f;
		
		SceneManager.LoadScene(gameOverSceneName);
    }
}
