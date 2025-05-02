using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IEffectable
{
    [SerializeField] private HintPanelController hintPanelController; 
    public Sprite openedSprite; 
    public GameObject wordObject; 
    public GameObject spriteChild;
    private Sprite initialSprite;
    private AudioSource interactableAudio;


    [Header("Effect Settings")]
    public bool requiresEffect = false; 
    public EffectBase requiredEffect; 

    [Header("Hint Messages")]
    [SerializeField] private string requiredEffectHintMessage = "I need to destroy this..";
    [SerializeField] private string interactHintMessage = "Press F to interact";
    
    [Header("State Settings")]
    public int interactableID;
    
    private SpriteRenderer spriteRenderer; 
    private bool isOpened = false;
    private bool isPlayerInRange = false; 
    private bool effectApplied = false; 

    void Start()
    {
        interactableAudio = GetComponent<AudioSource>();
        if (spriteChild != null)
        {
            spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                initialSprite = spriteRenderer.sprite;
            }
        }
        if (InteractableStateManager.IsInteractableOpened(interactableID))
        {
            ForceOpen();
        }
        else
        {
            isOpened = false;
            wordObject.SetActive(false);
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = initialSprite;
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isOpened && isPlayerInRange)
        {
            if (requiresEffect && !effectApplied)
            {
                hintPanelController.Show(requiredEffectHintMessage);
                return;
            }
            
            OpenObject();
        }
    }

    void OpenObject()
    {
        isOpened = true; 
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = openedSprite; 
        }
        wordObject.SetActive(true); 
        hintPanelController.Hide(); 
        InteractableStateManager.MarkInteractableOpened(interactableID);
        if (interactableAudio != null)
        {
            interactableAudio.Play();
        }
    }
    
    public void ForceOpen()
    {
        if (!isOpened)
        {
            isOpened = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = openedSprite;
            }
            wordObject.SetActive(true);
            hintPanelController.Hide();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            if (requiresEffect && !effectApplied)
            {
                hintPanelController.Show(requiredEffectHintMessage);
            }
            else
            {
                hintPanelController.Show(interactHintMessage); 
            }
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isOpened)
            {
                hintPanelController.Hide(); 
            }
            isPlayerInRange = false;
        }
    }
    
    public void ApplyEffect(EffectBase effect)
    {
        if (requiresEffect && effect == requiredEffect)
        {
            effectApplied = true;
            hintPanelController.Show(interactHintMessage); 
        }
    }
    
    public void ResetInteractableObject()
    {
        isOpened = false;
        effectApplied = false;
        wordObject.SetActive(false);
        hintPanelController.Hide();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = initialSprite;
        }
    }
    
    public bool CanReceiveEffect(Vector3 playerPosition, float effectRadius, EffectBase effect)
    {
        return Vector3.Distance(transform.position, playerPosition) <= effectRadius;
    }
}
