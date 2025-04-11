using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main menu objects")]
    [SerializeField] private GameObject loadingBarObject;
    [SerializeField] private Image loadingBar;
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Scene to load")] 
    [SerializeField] private string persistentGameplay = "Player&interactables";
    [SerializeField] private string levelScene = "SilverForest";
    
    private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        loadingBarObject.SetActive(false);
    }

    public void StartGame()
    {
        HideMenu();
        loadingBarObject.SetActive(true);
        
        sceneToLoad.Add(SceneManager.LoadSceneAsync(persistentGameplay));
        sceneToLoad.Add(SceneManager.LoadSceneAsync(levelScene, LoadSceneMode.Additive));
        StartCoroutine(ProgressLoadingBar());
    }

    private void HideMenu()
    {
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            objectsToHide[i].SetActive(false);
        }
    }

    private IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        for (int i = 0; i < sceneToLoad.Count; i++)
        {
            while (!sceneToLoad[i].isDone)
            {
                loadProgress += sceneToLoad[i].progress;
                loadingBar.fillAmount = loadProgress / sceneToLoad.Count;
                yield return null;
            }
        }
    }
}