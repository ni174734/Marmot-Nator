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

	public HUDController hudController;

    private bool quotaMet = false;
	
	private float runStartTime;

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
        runStartTime = Time.time;
		
		if (scoreManager == null)
            scoreManager = ScoreManager.Instance;
		
		if (hudController == null)
			hudController = FindFirstObjectByType<HUDController>();

        UpdateUI();
		ProceedToNextLevel();
    }
	
	public float GetTotalTimePlayed()
	{
		return Time.time - runStartTime;
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
		currentLevel++;
		ProceedToNextLevel();
		UpdateUI();
    }

    public void ProceedToNextLevel()
    {
        foodEatenThisLevel = 0;
        quotaMet = false;

        if (scoreManager != null)
		{
			scoreManager.food = 0;
		}

		if (hudController != null)
		{
			hudController.ResetTimer();
			//hudController.SetScore(0);
			hudController.SetFood(0);
		}
		
		if (player == null)
			player = FindFirstObjectByType<playerController>()?.transform;

		if (player != null)
		{
			playerController pc = player.GetComponent<playerController>();
			if (pc != null)
				pc.ResetMovementStats();
		}
		
		if (levelGenerator == null)
			levelGenerator = FindFirstObjectByType<LevelGenerator>();

		if (levelGenerator != null)
			levelGenerator.GenerateLevel(currentLevel);
		else
			Debug.LogError("No LevelGenerator found!");


        //Debug.Log("Loading gameplay scene: " + gameplaySceneName);
        //SceneManager.LoadScene(gameplaySceneName);
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