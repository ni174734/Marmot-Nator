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
	
	[Header("Director Difficulty")]
	public int extraFoodEatenLastLevel = 0;
	public int extraEnemyPerExtraFood = 1;
	public float enemySpeedIncreasePerExtraFood = 0.15f;
	public float maxEnemySpeedMultiplier = 2.5f;
	
	[Header("Transfer UI")]
	public GameObject transferCanvas;
	public TextMeshProUGUI transferMessageText;

	public HUDController hudController;

	private float totalPlayTime = 0f;
	private bool playTimerPaused = false;
	
	private float enemySpeedMultiplier = 1f;

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
		if (scoreManager == null)
            scoreManager = ScoreManager.Instance;
		
		if (hudController == null)
			hudController = FindFirstObjectByType<HUDController>();

        if (transferCanvas != null)
			transferCanvas.SetActive(false);
		
		UpdateUI();
		StartLevel();
		//ProceedToNextLevel();
    }
	
	private void Update()
	{
		if (!playTimerPaused)
			totalPlayTime += Time.deltaTime;
	}

	public float GetTotalTimePlayed()
	{
		return totalPlayTime;
	}
	
	public float GetEnemySpeedMultiplier()
	{
		return enemySpeedMultiplier;
	}
	
	public int GetExtraEnemyCount()
	{
		return extraFoodEatenLastLevel * extraEnemyPerExtraFood;
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
		int quota = GetCurrentQuota();
		
		int extraFood = foodEatenThisLevel - quota;
		
		if (extraFood < 0) extraFood = 0;
		
		extraFoodEatenLastLevel = extraFood;
		enemySpeedMultiplier = 1f + (extraFoodEatenLastLevel * enemySpeedIncreasePerExtraFood);
		
		if (enemySpeedMultiplier > maxEnemySpeedMultiplier) enemySpeedMultiplier = maxEnemySpeedMultiplier;
		
		playTimerPaused = true;
		
		if (levelGenerator != null) levelGenerator.ClearGeneratedLevel();
		if (transferMessageText != null) transferMessageText.text = "You passed level " + currentLevel;
		
		transferCanvas.SetActive(true);

		UpdateUI();
    }

    public void ProceedToNextLevel()
    {
        if (transferCanvas != null) transferCanvas.SetActive(false);
		
		currentLevel++;
		
		playTimerPaused = false;
		
		UpdateUI();
		StartLevel();
    }
	
	private void StartLevel()
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