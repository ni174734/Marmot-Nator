using UnityEngine;
using TMPro;

public class GameplaySceneBootstrap : MonoBehaviour
{
    public LevelGenerator levelGenerator;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI quotaText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI foodText;

    private void Start()
    {
        Debug.Log("GameplaySceneBootstrap Start");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL");
            return;
        }

        if (levelGenerator == null)
        {
            Debug.LogError("LevelGenerator is NOT assigned in GameplaySceneBootstrap");
            return;
        }

        GameManager.Instance.OnGameplaySceneLoaded(levelGenerator, levelText, quotaText);

        /*if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RebindUI(scoreText, foodText);
        }*/
    }
}