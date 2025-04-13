using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueActivator : MonoBehaviour, IInteractable, IEffectable
{
    public enum DialogueRequirement
    {
        NoCombo,
        QuietWhisper,
        EchoingRoots,
        WhisperingPetals
    }

    [SerializeField] private DialogueRequirement startRequirement = DialogueRequirement.NoCombo;
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
        if (CheckComboRequirements())
        {
            if(CompareTag("Lore"))
            {
                hintPanelController.Show("Press F to interact");
                return;
            }
            hintPanelController.Show("I can talk with them now");
        }
        else
        {
            hintPanelController.Show("I can't talk with them without right melody");
        }
    }

    public void Interact(Movement player)
    {
        if (!CheckComboRequirements())
        {
            Debug.Log("Cannot start dialogue — combo not met.");
            return;
        }

        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        hintPanelController.Hide(); 
        player.DialogueUI.ShowDialogue(dialogueObject);
        isDialogueActive = true;

        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueEnd += EndDialogue;
        }

        player.DisableMovement();  
    }

    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
        CheckEffectRequirements(effect.GetType().Name);
    }

    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            Debug.Log($"{effect.GetType().Name} applied to {gameObject.name}");
            CheckEffectRequirements(effect.GetType().Name);
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to DialogueActivator.");
        }
    }
    
    private void CheckEffectRequirements(string effectName)
    {
        Debug.Log($"Checking requirements for effect: {effectName}, Start Requirement: {startRequirement}");
        if ((startRequirement == DialogueRequirement.QuietWhisper && effectName == "QuietWhisperEffect") ||
            (startRequirement == DialogueRequirement.EchoingRoots && effectName == "EchoingRootsEffect") ||
            (startRequirement == DialogueRequirement.WhisperingPetals && effectName == "WhisperingPetalsEffect"))
        {
            EnableDialogueStart();
            Debug.Log($"Effect {effectName} matched requirement {startRequirement}, dialogue start enabled.");
        }
        else
        {
            Debug.LogWarning($"Effect {effectName} did not match requirement {startRequirement}.");
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
        Debug.Log($"Start requirement: {startRequirement}, Can Start Dialogue: {canStartDialogue}");

        switch (startRequirement)
        {
            case DialogueRequirement.NoCombo:
                return true;

            case DialogueRequirement.QuietWhisper:
            case DialogueRequirement.EchoingRoots:
            case DialogueRequirement.WhisperingPetals:
                return canStartDialogue;

            default:
                return false;
        }
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
}