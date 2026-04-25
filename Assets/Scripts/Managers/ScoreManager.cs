using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
	public int totalScore = 0;
    public int food = 0;

    [Header("Point Values")]
    public int healthyPoints = 150;
    public int junkPoints = 80;
    public int boostPoints = 200;

    [Header("UI")]
    [Tooltip("Drag UI text here.")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI foodText;

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
        UpdateUI();
    }

    public void AddFood(FoodItem.FoodType type)
    {
        int points = 0;

        switch (type)
        {
            case FoodItem.FoodType.Healthy:
                points = healthyPoints;
                break;

            case FoodItem.FoodType.Junk:
                points = junkPoints;
                break;

            case FoodItem.FoodType.Boost:
                points = boostPoints;
                break;
        }

        score += points;
        food += 1;

        UpdateUI();
    }

	private void UpdateScore()
    {
        totalScore += score;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}