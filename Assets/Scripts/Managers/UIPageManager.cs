using UnityEngine;

public class UIPageManager : MonoBehaviour
{
    public GameObject gameplayHUD;
    public GameObject leaderboardPanel;

    private void Start()
    {
        ShowGameplayHUD();
    }

    public void ShowLeaderboard()
    {
        gameplayHUD.SetActive(false);
        leaderboardPanel.SetActive(true);
    }

    public void ShowGameplayHUD()
    {
        leaderboardPanel.SetActive(false);
        gameplayHUD.SetActive(true);
    }
}