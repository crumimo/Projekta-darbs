using UnityEngine;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }
}