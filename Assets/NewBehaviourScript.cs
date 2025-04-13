using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
     [SerializeField] private Canvas targetCanvas; 
    [SerializeField] private string mainMenuSceneName = "MainMenu"; 
    [SerializeField] private FadeController fadeController; 
    [SerializeField] private float delayBeforeSceneChange = 3f; 

    private bool isActivated = false; 

    private void Start()
    {
        if (targetCanvas != null)
        {
            targetCanvas.enabled = false; 
        }
        else
        {
            Debug.LogWarning("Target Canvas is not assigned in the inspector.");
        }

        if (fadeController == null)
        {
            Debug.LogError("FadeController is not assigned in the inspector.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.CompareTag("Player")) 
        {
            isActivated = true; 
            StartCoroutine(ActivateCanvasAndLoadScene());
        }
    }

    private IEnumerator ActivateCanvasAndLoadScene()
    {
        if (fadeController != null)
        {
            yield return fadeController.FadeIn();
        }
        
        if (targetCanvas != null)
        {
            targetCanvas.enabled = true; 
        }
        
        if (fadeController != null)
        {
            yield return fadeController.FadeOut();
        }

        Debug.Log("Canvas activated. Transitioning to Main Menu in " + delayBeforeSceneChange + " seconds.");
        yield return new WaitForSeconds(delayBeforeSceneChange);
        
        if (fadeController != null)
        {
            yield return fadeController.FadeIn();
        }

        
        LoadMainMenuScene();
    }

    private void LoadMainMenuScene()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName); 
        }
        else
        {
            Debug.LogError("Main Menu scene name is not set.");
        }
    }
}
