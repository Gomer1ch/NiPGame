using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Linq;

public class GameUIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;

    public Button startButton;
    public Button exitButton;
    public Button restartButton;

    private void SetControlEnabled(bool enabled)
    {
        var controllables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameControl>();

        foreach (var control in controllables)
        {
            control.SetControlEnabled(enabled);
        }
    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);

        Time.timeScale = 0f; // Остановка игры до старта
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        SetControlEnabled(true);
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
        UnityEngine.Debug.Log("Выход из игры");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        SetControlEnabled(false);
    }
}
