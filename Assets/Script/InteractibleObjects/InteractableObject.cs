using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI text;
    public Sprite openedSprite; // Спрайт для открытого состояния
    public GameObject wordObject; // Объект слова, который будет активироваться
    public GameObject spriteChild; // Дочерний объект, содержащий SpriteRenderer

    private SpriteRenderer spriteRenderer; // Спрайт рендерер дочернего объекта
    private bool isOpened = false; // Флаг состояния (открыт или закрыт)
    private bool isPlayerInRange = false; // Флаг нахождения игрока в зоне взаимодействия

    void Start()
    {
        if (spriteChild != null)
        {
            spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
        }
        wordObject.SetActive(false); // Скроем объект слова при старте
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
            spriteRenderer.sprite = openedSprite; // Меняем спрайт дочернего объекта
        }
        wordObject.SetActive(true); // Отображаем объект слова
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            text.text = "Press F to interact";
            hintPanel.SetActive(true);
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintPanel.SetActive(false);
            text.text = "";
            isPlayerInRange = false;
        }
    }
}