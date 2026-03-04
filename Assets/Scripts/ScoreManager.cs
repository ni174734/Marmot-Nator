using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
	
	public int score = 0;
	
	[Header("UI")]
	[Tooltip("Drag UI text here.")]
	public TextMeshProUGUI scoreText;
	
	private void Awake()
	{
		if(Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
	
	public void AddPoint(int amount)
	{
		score += amount;
		
		if(scoreText != null)
			scoreText.text = "Score: " + score;
	}
}
