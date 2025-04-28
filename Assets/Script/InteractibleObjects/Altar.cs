using TMPro;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private HintPanelController hintPanelController; 
    [SerializeField] private string hintMessage = "Press F to interact";
    [SerializeField] private string firstInteractionMessage = "Words restored and map activated!"; 
    [SerializeField] private float messageDisplayDuration = 3f;
    
    [SerializeField] private int maxUses = 3; 
    private int currentUses = 0; 
    
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
            if (currentUses >= maxUses)
            {
                ShowHint("The altar has no power left.");
                return;
            }

            if (!hasShownFirstMessage)
            {
                ActivateCollectedWords(); 
                ActivateAllEnemies();
                mapManager.ActivateMap();
                WordUIManager.Instance.ClearCollectedWords(); 
                ShowHint(firstInteractionMessage + $" ({maxUses - currentUses - 1} uses left)");
                hasShownFirstMessage = true;
                Invoke(nameof(SwitchToHintMessage), messageDisplayDuration);
            }
            else
            {
                ActivateCollectedWords();
                ActivateAllEnemies();
                WordUIManager.Instance.ClearCollectedWords();
                ShowHint($"Words and enemies refreshed. ({maxUses - currentUses - 1} uses left)");
            }

            currentUses++;
        }

    }

    private void ActivateCollectedWords()
    {
        Debug.Log("Activating all collected words.");
        WordUIManager.Instance.ResetCollectedWords(); 
        
    }
    
    private void ActivateAllEnemies()
    {
        PatrolEnemy[] allEnemies = Resources.FindObjectsOfTypeAll<PatrolEnemy>();
        
        foreach (PatrolEnemy enemy in allEnemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                enemy.gameObject.SetActive(true);
            }
            
            if (enemy.permanentSleep)
            {
                enemy.ResetEnemy();
                EnemyStateManager.MarkEnemyAsAwake(enemy.enemyID); 
            }
            enemy.isAsleep = false;
        }
        
        Debug.Log($"Activated {allEnemies.Length} enemies.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (currentUses >= maxUses)
            {
                ShowHint("The altar has no power left.");
            }
            else
            {
                ShowHint($"{hintMessage} ({maxUses - currentUses} uses left)");
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