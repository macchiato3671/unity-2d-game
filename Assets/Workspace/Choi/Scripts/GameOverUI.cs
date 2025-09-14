using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        GameManager.inst.IsPaused = true;
        //Time.timeScale = 0f; // 게임 정지 (선택)
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        GameManager.inst.IsPaused = false;
        GameManager.inst.ChangeScene(0);
    }
}
