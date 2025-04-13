using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private float fadeDuration = 0.5f;
    
    [SerializeField] private FadeController fadeController;

    private CanvasGroup canvasGroup;
    private bool isPaused = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        HideInstant();
        settingsPanel.SetActive(false); 
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        settingsPanel.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(FadeIn());
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPaused = false;
        StopAllCoroutines();
        StartCoroutine(FadeOut());
        Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1; 
        StartCoroutine(GoToMainMenuWithFade());
    }

    private IEnumerator GoToMainMenuWithFade()
    {
        yield return StartCoroutine(fadeController.FadeIn());
        
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Time.timeScale = 1;
#if UNITY_EDITOR
        Debug.Log("Игра завершена (работает только в сборке)");
#else
        Application.Quit();
#endif
    }

    private void HideInstant()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void ResetToLastCheckpoint()
    {
        Time.timeScale = 1; // Resume normal time temporarily to apply changes

        // Retrieve the saved checkpoint data
        GameState gameState = GameSession.Instance.GameState;
        if (gameState.currentCheckpointID >= 0)
        {
            // Restore player position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = gameState.PlayerPosition;
            }
        
            // Restore obstacles and other game states
            CheckpointManager.RestoreObstacles();
            WordUIManager.Instance.RestoreCollectedWordsOnScene();
            ObstacleStateManager.RestoreCheckpointState();

            Debug.Log("Player reset to last checkpoint: " + gameState.currentCheckpointID);
        }
        else
        {
            Debug.LogWarning("No checkpoint data available to reset the player.");
        }

        // Resume the game and close the pause menu
        ResumeGame();
    }
}
