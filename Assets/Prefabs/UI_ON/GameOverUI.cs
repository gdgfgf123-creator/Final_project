using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private string mainMenuSceneName = "Main menu";

    private void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void Show()
    {
        ScoreTracker tracker = ScoreTracker.Instance;
        int finalScore = tracker != null ? tracker.Score : 0;

        if (finalScoreText != null)
            finalScoreText.text = $"Score: {finalScore}";

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}