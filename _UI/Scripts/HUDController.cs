using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text foodText;

    [Header("Game Values")]
    public float startTimeSeconds = 90f;

    private float timeLeft;
    private int score;
    private int food;

    void Start()
    {
        timeLeft = startTimeSeconds;
        RefreshAll();
    }

    void Update()
    {
        // Countdown
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0) timeLeft = 0;

        RefreshTimer();

        // (Optional) test controls in your UI-only project:
        // Press T to add score, Y to add food
        if (Input.GetKeyDown(KeyCode.T)) AddScore(10);
        if (Input.GetKeyDown(KeyCode.Y)) AddFood(1);
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        RefreshScore();
    }

    public void AddScore(int amount)
    {
        score += amount;
        RefreshScore();
    }

    public void SetFood(int newFood)
    {
        food = newFood;
        RefreshFood();
    }

    public void AddFood(int amount)
    {
        food += amount;
        RefreshFood();
    }

    public void ResetTimer()
    {
        timeLeft = startTimeSeconds;
        RefreshTimer();
    }

    public float GetTimeLeft() => timeLeft;

    private void RefreshAll()
    {
        RefreshTimer();
        RefreshScore();
        RefreshFood();
    }

    private void RefreshTimer()
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = $"Time: {seconds}";
    }

    private void RefreshScore()
    {
        scoreText.text = $"Score: {score}";
    }

    private void RefreshFood()
    {
        foodText.text = $"Food: {food}";
    }
}