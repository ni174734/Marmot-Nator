using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Change these later to match your team's scene names in the main project
    [Header("Scene Names")]
    public string gameSceneName = "Game";
    public string mainMenuSceneName = "MainMenu";
    public string controlsSceneName = "Controls";
    public string highScoresSceneName = "HighScores";
    public string gameOverSceneName = "GameOver";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenControls()
    {
        SceneManager.LoadScene(controlsSceneName);
    }

    public void OpenHighScores()
    {
        SceneManager.LoadScene(highScoresSceneName);
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void QuitGame()
    {
        // Quits the built game
        Application.Quit();

        // Makes the Quit button "do something" in the editor too
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}