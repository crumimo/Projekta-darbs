using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IEffectable
{
    [SerializeField] private HintPanelController hintPanelController; 
    public Sprite openedSprite; 
    public GameObject wordObject; 
    public GameObject spriteChild; 

    [Header("Effect Settings")]
    public bool requiresEffect = false; 
    public EffectBase requiredEffect; 

    [Header("Hint Messages")]
    [SerializeField] private string requiredEffectHintMessage = "I need to destroy this..";
    [SerializeField] private string interactHintMessage = "Press F to interact";
    
    private SpriteRenderer spriteRenderer; 
    private bool isOpened = false;
    private bool isPlayerInRange = false; 
    private bool effectApplied = false; 

    void Start()
    {
        if (spriteChild != null)
        {
            spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
        }
        wordObject.SetActive(false); 
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
    }

    void OnTriggerEnter2D(Collider2D other)
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

    void OnTriggerExit2D(Collider2D other)
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
    
    
}