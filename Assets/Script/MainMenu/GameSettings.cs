using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

   
    public int resolutionIndex = 0; 
    public bool vSyncEnabled = true; 

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