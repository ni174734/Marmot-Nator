using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level Info")]
    public int currentLevel = 1;
    public int foodEatenThisLevel = 0;
    public int baseFoodQuota = 3;

    [Header("References")]
    public Transform player;
    public LevelGenerator levelGenerator;
    public ScoreManager scoreManager;

    [Header("Scenes")]
    public string gameplaySceneName = "MainGame";
    //public string transferSceneName = "LevelTransfer";

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI quotaText;

    private bool quotaMet = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        UpdateUI();
    }

    public int GetCurrentQuota()
    {
        return baseFoodQuota + (currentLevel - 1);
    }

    public void RegisterFoodEaten(FoodItem.FoodType type)
    {
        foodEatenThisLevel++;

        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        if (scoreManager != null)
            scoreManager.AddFood(type);

        if (foodEatenThisLevel >= GetCurrentQuota())
            quotaMet = true;

        UpdateUI();
    }

    public bool HasMetQuota()
    {
        return quotaMet;
    }

    public void CompleteLevel()
    {
        //Debug.Log("Loading transfer scene: " + transferSceneName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ProceedToNextLevel()
    {
        currentLevel++;
        foodEatenThisLevel = 0;
        quotaMet = false;

        levelText = null;
        quotaText = null;
        levelGenerator = null;
        player = null;

        Debug.Log("Loading gameplay scene: " + gameplaySceneName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnGameplaySceneLoaded(LevelGenerator generatorInScene, TextMeshProUGUI newLevelText, TextMeshProUGUI newQuotaText)
    {
        levelGenerator = generatorInScene;
        levelText = newLevelText;
        quotaText = newQuotaText;

        /*if (scoreManager == null)
            scoreManager = ScoreManager.Instance;*/

        if (levelGenerator != null)
            levelGenerator.GenerateLevel(currentLevel);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Level: " + currentLevel;

        if (quotaText != null)
        {
            if (quotaMet)
                quotaText.text = "Return to spawn!";
            else
                quotaText.text = "Food: " + foodEatenThisLevel + " / " + GetCurrentQuota();
        }
    }
}