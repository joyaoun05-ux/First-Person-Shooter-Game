using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text highScoreUI;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;

    [Header("Scene")]
    [SerializeField] private string newGameScene = "SampleScene";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float delayBeforeAction = 0.2f;

    private void Start()
    {
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    public void StartNewGame()
    {
        StartCoroutine(PlaySoundAndLoadScene());
    }

    public void ExistApplication()
    {
        StartCoroutine(PlaySoundAndExit());
    }

    public void OpenHowToPlay()
    {
        PlayClickSound();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        PlayClickSound();

        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private IEnumerator PlaySoundAndLoadScene()
    {
        PlayClickSound();

        yield return new WaitForSecondsRealtime(delayBeforeAction);

        SceneManager.LoadScene(newGameScene);
    }

    private IEnumerator PlaySoundAndExit()
    {
        PlayClickSound();

        yield return new WaitForSecondsRealtime(delayBeforeAction);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogError("AudioSource or ClickSound not assigned!");
        }
    }
}