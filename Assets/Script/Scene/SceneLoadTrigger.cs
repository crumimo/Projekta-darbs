using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private string[] scenesToLoad; 
    [SerializeField] private string[] scenesToUnload;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            LoadScenes();
            UnloadScenes();
        }
    }

    private void LoadScenes()
    {
        foreach (string sceneName in scenesToLoad)
        {
            bool isSceneLoaded = false;
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == sceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 
            }
        }
    }

    private void UnloadScenes()
    {
        foreach (string sceneName in scenesToUnload)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == sceneName)
                {
                    SceneManager.UnloadSceneAsync(sceneName); 
                }
            }
        }
    }
}