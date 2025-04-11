using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private HintPanelController hintPanelController; 
    public Sprite openedSprite; 
    public GameObject wordObject; 
    public GameObject spriteChild; 

    private SpriteRenderer spriteRenderer; 
    private bool isOpened = false;
    private bool isPlayerInRange = false; 

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
            hintPanelController.Show("Press F to interact"); 
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
}