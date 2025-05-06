using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main menu objects")]
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject settingsPanel;   

    [Header("Scene to load")]
    [SerializeField] private string persistentGameplay = "Player&interactables";
    [SerializeField] private string levelScene = "SilverForest";

    [Header("Fade Controller")]
    [SerializeField] private FadeController fadeController;

    private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();
    private string mainMenuScene;

    private void Awake()
    {
        mainMenuScene = SceneManager.GetActiveScene().name;
        settingsPanel.SetActive(false); 
    }

    public void StartGame()
    {
        HideMenu();
        
        CheckpointManager.ResetCheckpoints();
        
        if (GameSession.Instance != null)
        {
            GameSession.Instance.GameState = new GameState
            {
                PlayerPosition = Vector3.zero,   
                currentCheckpointID = -1         
            };
        }

        AsyncOperation persistentLoad = SceneManager.LoadSceneAsync(persistentGameplay, LoadSceneMode.Additive);
        AsyncOperation levelLoad = SceneManager.LoadSceneAsync(levelScene, LoadSceneMode.Additive);

        persistentLoad.allowSceneActivation = false;
        levelLoad.allowSceneActivation = false;

        sceneToLoad.Add(persistentLoad);
        sceneToLoad.Add(levelLoad);

        StartCoroutine(LoadScenesWithFade());
    }


    public void OpenSettings()
    {
        SetObjectsActive(objectsToHide, false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        SetObjectsActive(objectsToHide, true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        
        Debug.Log("Игра завершена (работает только в сборке)!");
#else
        // Если игра запущена в сборке
        Application.Quit();
#endif
    }

    private void HideMenu()
    {
        SetObjectsActive(objectsToHide, false);
    }

    private void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(isActive);
        }
    }

    private IEnumerator LoadScenesWithFade()
    {
        while (true)
        {
            float progress = 0f;
            foreach (AsyncOperation op in sceneToLoad)
            {
                progress += Mathf.Clamp01(op.progress / 0.9f);
            }

            if (progress / sceneToLoad.Count >= 1f)
                break;

            yield return null;
        }
        
        yield return StartCoroutine(fadeController.FadeIn());

        foreach (AsyncOperation op in sceneToLoad)
        {
            op.allowSceneActivation = true;
        }
        
        while (!SceneManager.GetSceneByName(levelScene).isLoaded || !SceneManager.GetSceneByName(persistentGameplay).isLoaded)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelScene));

        yield return SceneManager.UnloadSceneAsync(mainMenuScene);
        
        yield return StartCoroutine(fadeController.FadeOut());
    }
}