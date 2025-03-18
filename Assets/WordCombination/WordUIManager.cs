using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WordUIManager : MonoBehaviour
{
    public static WordUIManager Instance;

    [Header("Panels")]
    public Canvas worldCanvas;
    public Transform playerTransform;

    [Header("Buttons")]
    public Button[] wordButtons;
    public Button[] topButtons;
    public Button confirmButton;

    [Header("Effect Radius")]
    public float effectRadius = 5f; // Радиус действия эффекта

    private List<string> selectedWords = new();
    private Dictionary<string, int> collectedWords = new();
    private Dictionary<string, int> savedCollectedWords = new(); // Для сохранения состояния слов при активации чекпоинта

    private List<WordCollector> trackedWords = new(); // Для отслеживания собранных слов
    private List<WordCollector> checkpointTrackedWords = new(); // Слова, сохраненные в чекпоинте

    private Movement playerMovement;
    private EnemyManager enemyManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        confirmButton.onClick.AddListener(ConfirmCombination);
        worldCanvas.enabled = false;

        // Добавляем слушатели для кнопок в верхней панели
        foreach (var btn in topButtons)
        {
            btn.onClick.AddListener(() => DeselectWord(btn));
        }
    }

    void Start()
    {
        playerMovement = FindObjectOfType<Movement>();
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ToggleWorldPanel();
        if (worldCanvas.enabled) worldCanvas.transform.position = playerTransform.position + new Vector3(0, 2, 0);
    }

    public void CollectWord(string word, int count)
    {
        if (collectedWords.ContainsKey(word)) collectedWords[word] += count;
        else collectedWords[word] = count;

        UpdateButtons();
    }

    public void TrackCollectedWord(WordCollector wordCollector)
    {
        trackedWords.Add(wordCollector); // Отслеживаем собранное слово
    }

    public void SaveCheckpoint()
    {
        checkpointTrackedWords = new List<WordCollector>(trackedWords); // Сохраняем слова в чекпоинте
        savedCollectedWords = new Dictionary<string, int>(collectedWords); // Сохраняем состояние слов
    }

    public void RestoreCollectedWordsOnScene()
    {
        // Восстанавливаем слова на сцену, если они не были сохранены в чекпоинте
        foreach (var wordCollector in trackedWords)
        {
            if (!checkpointTrackedWords.Contains(wordCollector))
            {
                wordCollector.ResetWord();
            }
        }
        trackedWords.Clear(); // Очищаем список отслеживаемых слов
    }

    void ToggleWorldPanel()
    {
        worldCanvas.enabled = !worldCanvas.enabled;
        
        if (worldCanvas.enabled)
        {
            playerMovement.DisableMovement();
            enemyManager.PauseEnemies();
        }
        else
        {
            playerMovement.EnableMovement();
            enemyManager.ResumeEnemies();
        }
        
        if (!worldCanvas.enabled) ResetSelection();
    }

    void UpdateButtons()
    {
        int i = 0;
        foreach (var pair in collectedWords)
        {
            if (i < wordButtons.Length)
            {
                wordButtons[i].gameObject.SetActive(true);
                wordButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{pair.Key} x{pair.Value}";
                int index = i; // Capture the current index
                wordButtons[i].onClick.RemoveAllListeners(); // Remove any existing listeners
                wordButtons[i].onClick.AddListener(() => SelectWord(pair.Key));
            }
            i++;
        }
    }

    void SelectWord(string word)
    {
        if (selectedWords.Count < 2 && collectedWords[word] > 0)
        {
            selectedWords.Add(word);
            collectedWords[word]--;
            UpdateTopButtons();
            UpdateButtons();
        }
    }

    void DeselectWord(Button button)
    {
        string word = button.GetComponentInChildren<TextMeshProUGUI>().text;
        selectedWords.Remove(word);
        collectedWords[word]++;
        UpdateTopButtons();
        UpdateButtons();
    }

    void UpdateTopButtons()
    {
        for (int i = 0; i < topButtons.Length; i++)
        {
            if (i < selectedWords.Count)
            {
                topButtons[i].gameObject.SetActive(true);
                topButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = selectedWords[i];
            }
            else
            {
                topButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void ConfirmCombination()
    {
        if (selectedWords.Count == 2)
        {
            string effect = WordManager.Instance.GetEffect(selectedWords[0], selectedWords[1]);
            Debug.Log($"Confirmed combination: {selectedWords[0]} + {selectedWords[1]} = {effect}");

            if (AnyObjectInRange())
            {
                // Apply the effect to all enemies and dialogue triggers in range
                foreach (var enemy in FindObjectsOfType<PatrolEnemy>())
                {
                    enemy.ApplyEffect(effect);
                }
                foreach (var dialogueTrigger in FindObjectsOfType<DialogueTrigger>())
                {
                    dialogueTrigger.ApplyEffect(effect);
                }

                ResetSelection();
                // Закрываем панель после подтверждения комбинации
                worldCanvas.enabled = false;
                playerMovement.EnableMovement();
                enemyManager.ResumeEnemies();
            }
            else
            {
                Debug.Log("No objects in range to apply the effect.");
                // Возвращаем слова игроку, так как эффекта нет
                collectedWords[selectedWords[0]]++;
                collectedWords[selectedWords[1]]++;
                ResetSelection();
            }
        }
    }

    bool AnyObjectInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerTransform.position, effectRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PatrolEnemy>() != null || collider.GetComponent<DialogueTrigger>() != null)
            {
                return true;
            }
        }
        return false;
    }

    void ResetSelection()
    {
        selectedWords.Clear();
        foreach (var btn in topButtons) btn.gameObject.SetActive(false);
        UpdateButtons();
    }

    public void RestoreCollectedWords()
    {
        collectedWords = new Dictionary<string, int>(savedCollectedWords);
        UpdateButtons();
        ResetSelection();
    }

    public void ResetToCheckpoint()
    {
        RestoreCollectedWords();
    }
}