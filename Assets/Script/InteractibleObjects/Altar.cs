using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private GameObject sign;
    private bool isPlayerInRange = false; // Flag to track if the player is in the interaction zone
    private MapManager mapManager; // Reference to the MapManager

    private void Start()
    {
        mapManager = FindObjectOfType<MapManager>(); // Find the MapManager in the scene
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInRange)
        {
            ActivateCollectedWords();
            mapManager.ActivateMap(); // Activate the map when interacting with the altar
        }
    }

    void ActivateCollectedWords()
    {
        Debug.Log("Activating all collected words.");
        WordUIManager.Instance.ResetCollectedWords();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sign.SetActive(true);
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sign.SetActive(false);
            isPlayerInRange = false;
        }
    }
}