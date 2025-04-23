using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueActivator : MonoBehaviour, IInteractable, IEffectable
{
    [Header("Effect Settings")]
    public bool requiresEffect = false; 
    public EffectBase requiredEffect; 
    
    private bool effectApplied = false;
    
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private HintPanelController hintPanelController; 
    private bool canStartDialogue = false;
    private bool playerInRange = false;

    [SerializeField] private float distanceToActivate = 2f;
    private bool isDialogueActive = false;
    

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isDialogueActive)
        {
            TryStartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Movement player))
        {
            player.Interactable = this;
            playerInRange = true;
            UpdateTalkIndicators();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Movement player))
        {
            if (player != null && player.Interactable == this)
            {
                player.Interactable = null;
            }

            hintPanelController.Hide(); 
            playerInRange = false;
        }
    }

    private void UpdateTalkIndicators()
    {
        if (requiresEffect && !effectApplied)
        {
            hintPanelController.Show("I need a specific effect to communicate.");
            return;
        }

        if (CheckComboRequirements())
        {
            hintPanelController.Show("I can talk with them now");
        }
        else
        {
            hintPanelController.Show("I can't talk with them without the right melody.");
        }
    }

    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
        CheckEffectRequirements(effect);
    }

    private void CheckEffectRequirements(EffectBase appliedEffect)
    {
        if (requiresEffect && requiredEffect != null && appliedEffect == requiredEffect)
        {
            effectApplied = true;  
            EnableDialogueStart();
            Debug.Log($"Effect {appliedEffect.name} matched the required effect. Dialogue start enabled.");
        }
        else
        {
            Debug.LogWarning($"Applied effect {appliedEffect.name} did not match the required effect.");
        }
    }

    public void EnableDialogueStart()
    {
        canStartDialogue = true;
        UpdateTalkIndicators();
        Debug.Log("Dialogue can be started by pressing F.");
    }

    private void TryStartDialogue()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far to start dialogue.");
            return;
        }

        if (CheckComboRequirements())
        {
            StartDialogue();
        }
        else
        {
            Debug.Log("Combo requirement not met. Dialogue won't start.");
        }
    }

    private bool CheckComboRequirements()
    {
        if (requiresEffect && !effectApplied)
        {
            return false;
        }

        return canStartDialogue;
    }


    private void StartDialogue()
    {
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(dialogueObject);
            isDialogueActive = true;
            dialogueUI.OnDialogueEnd += EndDialogue;

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
            if (player != null)
            {
                player.DisableMovement();  
            }
        }
        canStartDialogue = false;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueEnd -= EndDialogue;
        }

        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
        if (player != null)
        {
            player.EnableMovement();  
        }
        UpdateTalkIndicators();
    }
    
    public void Interact(Movement player)
    {
        if (requiresEffect && !effectApplied)
        {
            Debug.Log("Cannot start dialogue – required effect not applied.");
            hintPanelController.Show("I need a specific effect to communicate.");
            return;
        }
        TryStartDialogue();
    }

}