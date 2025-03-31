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
    [SerializeField] private SpriteRenderer playerVisual;

    [Header("Buttons")]
    public Button[] wordButtons;
    public Button[] topButtons;
    public Button confirmButton;

    [Header("Effect Radius")]
    public float effectRadius = 10f;

    [Header("Effect Manager")]
    public EffectManager effectManager;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] collectionSounds; // Array of collection sounds
    private AudioSource audioSource;

    private List<string> selectedWords = new();
    private Dictionary<string, int> collectedWords = new();
    private Dictionary<string, int> savedCollectedWords = new();

    private List<WordCollector> trackedWords = new();
    private List<WordCollector> checkpointTrackedWords = new();
    private List<ObstacleManager> trackedObstacles = new(); // Track obstacles
    private List<ObstacleManager> checkpointTrackedObstacles = new(); // Track obstacles at checkpoint

    private Movement playerMovement;
    private EnemyManager enemyManager;

    [Header("Effects Folder")]
    public string effectsFolder = "Effects"; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        confirmButton.onClick.AddListener(ConfirmCombination);
        worldCanvas.enabled = false;

        foreach (var btn in topButtons)
        {
            btn.onClick.AddListener(() => DeselectWord(btn));
        }
    }

    void Start()
    {
        playerMovement = FindObjectOfType<Movement>();
        enemyManager = FindObjectOfType<EnemyManager>();

        if (effectManager == null)
        {
            Debug.LogError("EffectManager is not assigned!");
        }
        else
        {
            Debug.Log("EffectManager assigned successfully.");
        }
        foreach (var fish in FindObjectsOfType<Fish>())
        {
            Debug.Log("Fish found at position: " + fish.transform.position + ", with tag: " + fish.tag);
        }
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

        PlayRandomCollectionSound();
        UpdateButtons();
    }

    public void TrackCollectedWord(WordCollector wordCollector)
    {
        trackedWords.Add(wordCollector);
    }

    public void TrackObstacle(ObstacleManager obstacleManager)
    {
        trackedObstacles.Add(obstacleManager);
    }

    public void SaveCheckpoint()
    {
        checkpointTrackedWords = new List<WordCollector>(trackedWords);
        checkpointTrackedObstacles = new List<ObstacleManager>(trackedObstacles);
        savedCollectedWords = new Dictionary<string, int>(collectedWords);
        Debug.Log("Checkpoint saved: " + string.Join(", ", savedCollectedWords));
        ObstacleStateManager.SaveCheckpoint(); // Save the state of destroyed obstacles
    }

    public void RestoreCollectedWordsOnScene()
    {
        foreach (var wordCollector in trackedWords)
        {
            if (!checkpointTrackedWords.Contains(wordCollector))
            {
                wordCollector.ResetWord();
            }
        }
        trackedWords.Clear();
    }

    void ToggleWorldPanel()
    {
        worldCanvas.enabled = !worldCanvas.enabled;

        if (worldCanvas.enabled)
        {
            playerVisual.sortingLayerName = "Foreground";
            playerVisual.sortingOrder = 90;
            playerMovement.DisableMovement();
            enemyManager.PauseEnemies();

            // Hide top buttons when the panel is first opened
            foreach (var btn in topButtons)
            {
                btn.gameObject.SetActive(false);
            }
        }
        else
        {
            playerVisual.sortingLayerName = "Middleground";
            playerVisual.sortingOrder = 0;
            playerMovement.EnableMovement();
            enemyManager.ResumeEnemies();

            foreach (var word in selectedWords)
            {
                if (collectedWords.ContainsKey(word))
                {
                    collectedWords[word]++;
                }
                else
                {
                    collectedWords[word] = 1;
                }
            }
            ResetSelection();
        }
    }

    void PlayRandomCollectionSound()
    {
        if (collectionSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, collectionSounds.Length);
            audioSource.PlayOneShot(collectionSounds[randomIndex]);
        }
    }

    public void UpdateButtons() // Made public
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
        if (selectedWords.Count < 2 && collectedWords.ContainsKey(word) && collectedWords[word] > 0)
        {
            if (!selectedWords.Contains(word))
            {
                selectedWords.Add(word);
                collectedWords[word]--;
                UpdateTopButtons();
                UpdateButtons();
            }
        }
    }

    void DeselectWord(Button button)
    {
        string word = button.GetComponentInChildren<TextMeshProUGUI>().text;
        if (!string.IsNullOrEmpty(word))
        {
            selectedWords.Remove(word);
            if (collectedWords.ContainsKey(word))
            {
                collectedWords[word]++;
            }
            else
            {
                collectedWords[word] = 1;
            }
            UpdateTopButtons();
            UpdateButtons();
        }
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
                topButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                topButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void ConfirmCombination()
    {
        if (selectedWords.Count == 2)
        {
            EffectBase effect = WordManager.Instance.GetEffect(selectedWords[0], selectedWords[1]);
            if (effect == null)
            {
                Debug.LogError("Effect not found for the given combination.");
                return;
            }

            Debug.Log($"Confirmed combination: {selectedWords[0]} + {selectedWords[1]} = {effect.name}");
            string word1 = selectedWords[0];
            string word2 = selectedWords[1];

            bool objectsInRange = AnyObjectInRange();

            if (objectsInRange)
            {
                if (effect is SpikeCircleEffect)
                {
                    effect.Apply(playerTransform.gameObject);
                    Debug.Log($"Applying SpikeCircleEffect to Player");
                }
                else
                {
                    foreach (var enemy in FindObjectsOfType<PatrolEnemy>())
                    {
                        if (Vector3.Distance(playerTransform.position, enemy.transform.position) <= effectRadius)
                        {
                            effect.Apply(enemy.gameObject);
                        }
                    }
                    foreach (var dialogueTrigger in FindObjectsOfType<DialogueTrigger>())
                    {
                        if (Vector3.Distance(playerTransform.position, dialogueTrigger.transform.position) <= effectRadius)
                        {
                            effect.Apply(dialogueTrigger.gameObject);
                        }
                    }
                    foreach (var obstacleManager in FindObjectsOfType<ObstacleManager>())
                    {
                        if (Vector3.Distance(playerTransform.position, obstacleManager.transform.position) <= effectRadius)
                        {
                            effect.Apply(obstacleManager.gameObject);
                        }
                    }
                    foreach (var fish in FindObjectsOfType<Fish>())
                    {
                        if (Vector3.Distance(playerTransform.position, fish.transform.position) <= effectRadius && fish.CompareTag("Fish"))
                        {
                            Debug.Log("Applying FishBehavior to fish at position: " + fish.transform.position);
                            effect.Apply(fish.gameObject);
                        }
                    }
                }

                ResetSelection();

                worldCanvas.enabled = false;
                playerMovement.EnableMovement();
                enemyManager.ResumeEnemies();
            }
            else
            {
                Debug.Log("No objects in range to apply the effect.");

                if (collectedWords.ContainsKey(selectedWords[0]))
                {
                    collectedWords[selectedWords[0]]++;
                }
                else
                {
                    collectedWords[selectedWords[0]] = 1;
                }

                if (collectedWords.ContainsKey(selectedWords[1]))
                {
                    collectedWords[selectedWords[1]]++;
                }
                else
                {
                    collectedWords[selectedWords[1]] = 1;
                }

                ResetSelection();
            }
        }
    }

    bool AnyObjectInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerTransform.position, effectRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PatrolEnemy>() != null ||
                collider.GetComponent<DialogueTrigger>() != null ||
                collider.GetComponent<ObstacleManager>() != null)
            {
                return true;
            }
        }
        return false;
    }

    void ResetSelection()
    {
        selectedWords.Clear();
        foreach (var btn in topButtons)
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            btn.gameObject.SetActive(false);
        }
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
        ResetCollectedWords();
        ObstacleStateManager.RestoreCheckpointState(); // Restore obstacles to the checkpoint state
        ResetTrackedObstacles(); // Reset tracked obstacles
        ResetUICollectedWords(); // Clear the UI word buttons
    }

    public void ResetCollectedWords()
    {
        foreach (var wordCollector in trackedWords)
        {
            if (!checkpointTrackedWords.Contains(wordCollector))
            {
                wordCollector.ResetWord();
            }
        }
        trackedWords.Clear();
    }

    public void ResetTrackedObstacles()
    {
        foreach (var obstacleManager in trackedObstacles)
        {
            if (!checkpointTrackedObstacles.Contains(obstacleManager))
            {
                obstacleManager.ResetObstacle();
            }
        }
        trackedObstacles.Clear();
    }

    public void ResetUICollectedWords()
    {
        collectedWords.Clear();
        foreach (var btn in wordButtons)
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            btn.gameObject.SetActive(false);
        }
        UpdateButtons();
        ResetSelection();
    }
}