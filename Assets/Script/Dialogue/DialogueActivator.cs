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
    
    [Header("Dialogue NPC Settings")]
    public GameObject dialogueNpcPrefab;  
    public Transform dialogueNpcContainer;
    private GameObject currentNpcInstance;

    [SerializeField] private float distanceToActivate = 2f;
    private bool isDialogueActive = false;
    
    private string currentHint = "";
    public AudioClip dialogueMusicClip;

    
    private void Start()
    {
        if (!requiresEffect)
        {
            canStartDialogue = true;
        }
    }
    
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
            UpdateTalkIndicators(true); 
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
            
            currentHint = "";
        }
    }
    
    private void UpdateTalkIndicators(bool forceUpdate = false)
    {
        string newHint = "";
    
        if (requiresEffect && !effectApplied)
        {
            newHint = "I need a specific effect to communicate.";
        }
        else if (CheckComboRequirements())
        {
            newHint = "I can talk with them now";
        }
        else
        {
            newHint = "I can't talk with them without the right melody.";
        }
    
        if(!forceUpdate && newHint == currentHint)
        {
            return;
        }
    
        currentHint = newHint;
        hintPanelController.Show(newHint);
    }
    
    public bool ApplyEffect(EffectBase effect)
    {
        if (!requiresEffect)
        {
            StartCoroutine(ShowTemporaryHint("Nothing happened", 2f));
            return false;
        }
        effect.Apply(gameObject);
        CheckEffectRequirements(effect);
        return true;
    }
    
    private IEnumerator ShowTemporaryHint(string message, float delay)
    {
        hintPanelController.Show(message);
        yield return new WaitForSeconds(delay);
        hintPanelController.Hide();
    }

    private void CheckEffectRequirements(EffectBase appliedEffect)
    {
        if (requiresEffect && requiredEffect != null && appliedEffect == requiredEffect)
        {
            effectApplied = true;  
        }
        UpdateTalkIndicators();
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
    }

    private bool CheckComboRequirements()
    {
        return !requiresEffect || effectApplied;
    }

    private void StartDialogue()
    {
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            SoundManager.Instance.StartDialogue(dialogueMusicClip);
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
        
        if (dialogueNpcPrefab != null && dialogueNpcContainer != null)
        {
            if (currentNpcInstance != null)
            {
                Destroy(currentNpcInstance);
            }
            
            currentNpcInstance = Instantiate(dialogueNpcPrefab, dialogueNpcContainer);
            
            RectTransform rect = currentNpcInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }
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
        if (currentNpcInstance != null)
        {
            Destroy(currentNpcInstance);
        }
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
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        return Vector3.Distance(transform.position, playerPosition) <= effectRadius;
    }
}