using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable, IEffectable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private bool requiresNoCombo = false; 
    [SerializeField] private bool requiresQuietWhisper = false; 
    [SerializeField] private bool requiresEchoingRoots = false; 
    private bool canStartDialogue = false;
    private bool playerInRange = false;

    [SerializeField] private GameObject canTalk;
    [SerializeField] private GameObject cantTalk;

    [SerializeField] private float distanceToActivate;
    private bool isDialogueActive = false;
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void Update()
    {
        if (playerInRange && (canStartDialogue || requiresNoCombo) && Input.GetKeyDown(KeyCode.F) && !isDialogueActive)
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

            if (canStartDialogue || requiresNoCombo)
            {
                canTalk.SetActive(true);
                cantTalk.SetActive(false);
            }

            if (!canStartDialogue)
            {
                cantTalk.SetActive(true);
                canTalk.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Movement player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
            }
            playerInRange = false;

            cantTalk.SetActive(false);
            canTalk.SetActive(false);
        }
    }

    public void Interact(Movement player)
    {
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
    }
    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
    }
    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            Debug.Log($"{effect.GetType().Name} applied to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to DialogueActivator.");
        }
    }

    public void EnableDialogueStart()
    {
        canStartDialogue = true;
        Debug.Log("Dialogue can be started by pressing F.");
    }

    private void TryStartDialogue()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far away to start the dialogue.");
            return;
        }

        StartDialogue();
    }

    private void StartDialogue()
    {
        if (!requiresNoCombo && !canStartDialogue)
        {
            Debug.Log("Combination is required to start this dialogue.");
            return;
        }

        Debug.Log("Starting dialogue: " + dialogueObject.name);
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(dialogueObject);
            isDialogueActive = true; // Устанавливаем флаг в true, когда диалог начинается
            dialogueUI.OnDialogueEnd += EndDialogue; // Подписываемся на событие завершения диалога
            Debug.Log("Dialogue started.");
        }
        else
        {
            Debug.LogError("DialogueManager not found in the scene.");
        }
        canStartDialogue = false;
    }
    private void EndDialogue()
    {
        isDialogueActive = false; // Сбрасываем флаг, когда диалог завершается
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueEnd -= EndDialogue; // Отписываемся от события завершения диалога
        }
    }
}