using UnityEngine;
using TMPro;

public class LevelTransferUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    private void Start()
    {
        if (messageText != null)
        {
            if (GameManager.Instance != null)
                messageText.text = "You passed level " + GameManager.Instance.currentLevel;
            else
                messageText.text = "You passed the level!";
        }
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