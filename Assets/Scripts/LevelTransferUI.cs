using UnityEngine;
using TMPro;

public class LevelTransferUI : MonoBehaviour
{
    public TMP_Text messageText;

    private void Start()
    {
		Debug.Log("Transfer scene loaded. GM = " + (GameManager.Instance ? GameManager.Instance : "NULL"));
		
		if (messageText != null && GameManager.Instance != null)
			messageText.text = "You passed level " + GameManager.Instance.currentLevel;
    }

    public void Proceed()
    {
        Debug.Log("Proceed button pressed");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL in transfer scene");
            return;
        }

        GameManager.Instance.ProceedToNextLevel();
    }
}