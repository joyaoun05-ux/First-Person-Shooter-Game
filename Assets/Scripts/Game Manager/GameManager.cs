using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Player References")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Stop player movement / look
        if (firstPersonController != null)
            firstPersonController.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.move = Vector2.zero;
            starterAssetsInputs.look = Vector2.zero;
            starterAssetsInputs.jump = false;
            starterAssetsInputs.sprint = false;
        }

        // Unlock and show mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game
        Time.timeScale = 0f;

        // Show UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null && ScoreManager.Instance != null)
            finalScoreText.text = "Final Score: " + ScoreManager.Instance.GetScore();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }
}