using TMPro;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private HintPanelController hintPanelController; 
    [SerializeField] private string hintMessage = "Press F to interact";
    [SerializeField] private string firstInteractionMessage = "Words and map activated!"; 
    [SerializeField] private float messageDisplayDuration = 3f; 
    private bool isPlayerInRange = false; 
    private bool hasShownFirstMessage = false; 
    private string currentHintText = ""; 
    private MapManager mapManager; 

    private void Start()
    {
        mapManager = FindObjectOfType<MapManager>(); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInRange)
        {
            if (!hasShownFirstMessage)
            {
                ActivateCollectedWords();
                mapManager.ActivateMap(); 
                ShowHint(firstInteractionMessage); 
                hasShownFirstMessage = true; 
                Invoke(nameof(SwitchToHintMessage), messageDisplayDuration); 
            }
            else
            {
                ActivateCollectedWords(); 
                Debug.Log("Words refreshed.");
            }
        }
    }

    private void ActivateCollectedWords()
    {
        Debug.Log("Activating all collected words.");
        WordUIManager.Instance.ResetCollectedWords(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (!hasShownFirstMessage)
            {
                ShowHint(hintMessage); 
            }
            else
            {
                ShowHint(hintMessage); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            
            HideHintPanel();
        }
    }

    private void ShowHint(string message)
    {
        currentHintText = message;
        hintPanelController.Show(message);
    }

    private void HideHintPanel()
    {
        hintPanelController.Hide();
        currentHintText = ""; 
    }

    private void SwitchToHintMessage()
    {
        if (isPlayerInRange) 
        {
            ShowHint(hintMessage);
        }
        else
        {
            HideHintPanel();
        }
    }
}