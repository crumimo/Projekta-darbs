using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    public GameState GameState = new GameState();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameState.currentCheckpointID != -1)
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            playerTransform.position = GameState.PlayerPosition;
            Debug.Log("Player repositioned to last checkpoint: " + GameState.currentCheckpointID);

            // Восстанавливаем собранные слова
            WordUIManager.Instance.ResetToCheckpoint();
        }
    }
}