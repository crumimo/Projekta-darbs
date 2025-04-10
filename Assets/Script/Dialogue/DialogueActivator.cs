using TMPro;
using UnityEngine;

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
    private bool canStartDialogue = false;
    private bool playerInRange = false;

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float distanceToActivate = 2f;
    private bool isDialogueActive = false;

    [Header("Effect Diary Entries")]
    public EffectDiaryEntry[] effectDiaryEntries;

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

            if (hintPanel != null) 
            {
                hintPanel.SetActive(false);
            }

            if (text != null) 
            {
                text.text = "";
            }

            playerInRange = false;
        }
    }

    private void UpdateTalkIndicators()
    {
        if (CheckComboRequirements())
        {
            text.text = "I can talk with them now";
            hintPanel.SetActive(true);
        }
        else
        {
            text.text = "I can't talk with them without right melody";
            hintPanel.SetActive(true);
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
        
        hintPanel.SetActive(false);
        text.text = "";
        player.DialogueUI.ShowDialogue(dialogueObject);
        isDialogueActive = true;

        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueEnd += EndDialogue;
        }

        player.DisableMovement();  // Остановить движение игрока
    }

    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
        AddNotebookEntry(effect);
        CheckEffectRequirements(effect.GetType().Name);
    }

    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            AddNotebookEntry(effect);
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
        if ((startRequirement == DialogueRequirement.QuietWhisper && effectName == "QuietWhisperEffect") ||
            (startRequirement == DialogueRequirement.EchoingRoots && effectName == "EchoingRootsEffect") ||
            (startRequirement == DialogueRequirement.WhisperingPetals && effectName == "WhisperingPetalsEffect"))
        {
            EnableDialogueStart();
        }
    }

    private void AddNotebookEntry(ScriptableObject effect)
    {
        foreach (var entry in effectDiaryEntries)
        {
            if (entry.effectName == effect.GetType().Name)
            {
                NotebookManager.Instance.AddEntry(entry.effectName, entry.diaryEntryTemplate);
                Debug.Log($"Notebook entry added for effect: {entry.effectName}");
                break;
            }
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
                player.DisableMovement();  // Остановить движение игрока
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
            player.EnableMovement();  // Возобновить движение игрока
        }
        UpdateTalkIndicators();
    }
}