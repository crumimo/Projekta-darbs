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
        gameObject.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        gameObject.SetActive(true);
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
}
