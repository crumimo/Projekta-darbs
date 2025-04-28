using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

public class WordUIManager : MonoBehaviour
{
    public static WordUIManager Instance;

    [Header("Panels")]
    public Canvas worldCanvas;
    public CanvasGroup worldCanvasGroup; 
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
    [SerializeField] private AudioClip[] collectionSounds; 
    private AudioSource audioSource;

    [Header("Combination Icon Manager")]
    public CombinationIconManager combinationIconManager; 
    
    private List<string> selectedWords = new();
    private Dictionary<string, int> collectedWords = new();
    private Dictionary<string, int> savedCollectedWords = new();

    private List<WordCollector> trackedWords = new();
    private List<WordCollector> checkpointTrackedWords = new();
    private List<ObstacleManager> trackedObstacles = new(); 
    private List<ObstacleManager> checkpointTrackedObstacles = new(); 

    private Movement playerMovement;
    private EnemyManager enemyManager;

    [SerializeField] private Vector3 UIOffset;

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
        
        RestoreCollectedWords();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ToggleWorldPanel();
        if (worldCanvas.enabled) worldCanvas.transform.position = playerTransform.position + new Vector3(UIOffset.x, UIOffset.y, UIOffset.z);
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
        ObstacleStateManager.SaveCheckpoint(); 
    }

    public void RestoreCollectedWordsOnScene()
    {
        foreach (var wordCollector in trackedWords)
        {
            wordCollector.gameObject.SetActive(true);
            wordCollector.ResetWord();
        }
        foreach (var wordCollector in checkpointTrackedWords)
        {
            if (wordCollector != null)
            {
                wordCollector.gameObject.SetActive(true);
                wordCollector.ResetWord();
            }
        }
        trackedWords = checkpointTrackedWords.Concat(trackedWords).Distinct().ToList();
    
        collectedWords = new Dictionary<string, int>(savedCollectedWords);
        UpdateButtons();
    }



    void ToggleWorldPanel()
    {
        enemyManager.PauseEnemies();
        playerMovement.DisableMovement();
        if (worldCanvas.enabled)
        {
            StartCoroutine(FadeOut(worldCanvasGroup, 0.3f, () =>
            {
                worldCanvas.enabled = false;
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
                combinationIconManager.ResetCombinationIcon();
                ResetSelection();
            }));
        }
        else
        {
            worldCanvas.enabled = true;
            StartCoroutine(FadeIn(worldCanvasGroup, 0.3f, () =>
            {
                playerVisual.sortingLayerName = "Foreground";
                playerVisual.sortingOrder = 90;
                playerMovement.DisableMovement();
                enemyManager.PauseEnemies();
                
                foreach (var btn in topButtons)
                {
                    btn.gameObject.SetActive(false);
                }
            }));
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup, float duration, Action onComplete = null)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        if (onComplete != null)
        {
            onComplete();
        }
    }
    
    IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, Action onComplete = null)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        if (onComplete != null)
        {
            onComplete();
        }
    }

    void PlayRandomCollectionSound()
    {
        if (collectionSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, collectionSounds.Length);
            audioSource.PlayOneShot(collectionSounds[randomIndex]);
        }
    }

    public void UpdateButtons()
    {
        foreach (var btn in wordButtons)
        {
            btn.gameObject.SetActive(false); // Deactivate all buttons initially
        }

        int i = 0;
        foreach (var pair in collectedWords)
        {
            if (i < wordButtons.Length)
            {
                wordButtons[i].gameObject.SetActive(true); // Ensure button is active
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

        if (selectedWords.Count == 2 && !string.IsNullOrEmpty(selectedWords[0]) && !string.IsNullOrEmpty(selectedWords[1]))
        {
            string word1 = selectedWords[0];
            string word2 = selectedWords[1];
            combinationIconManager.DisplayCombinationIcon(word1, word2);
        }
        else
        {
            combinationIconManager.DisplayCombinationIcon(null, null); 
        }
    }

    
    private void PlayEffectSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void ConfirmCombination()
    {
        if (selectedWords.Count == 2)
        {
            EffectApplicationManager.Instance.ApplyCombinationEffect(
                selectedWords,
                onEffectApplied: () =>
                {
                    StartCoroutine(FadeOut(worldCanvasGroup, 0.5f, () =>
                    {
                        playerVisual.sortingLayerName = "Middleground";
                        playerVisual.sortingOrder = 0;
                        worldCanvas.enabled = false;
                        playerMovement.EnableMovement();
                        enemyManager.ResumeEnemies();
                    }));
                },
                onEffectFailed: () =>
                {
                    ReturnWordsToCollection();
                }
            );
            ResetSelection();
        }
    }
    
    private void ReturnWordsToCollection()
    {
    
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

        Debug.Log("Words returned to inventory.");
        UpdateButtons();
    }

    bool AnyObjectInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerTransform.position, effectRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<IEffectable>() != null)
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
        ActivateCollectedWordsUI(); 
    }

    public void ActivateCollectedWordsUI()
    {
        foreach (var pair in collectedWords)
        {
            if (pair.Value > 0)
            {
                foreach (var btn in wordButtons)
                {
                    if (btn.GetComponentInChildren<TextMeshProUGUI>().text.Contains(pair.Key))
                    {
                        btn.gameObject.SetActive(true); 
                    }
                }
            }
        }
    }

    public void ResetToCheckpoint()
    {
        RestoreCollectedWords();
        RestoreCollectedWordsOnScene();
        ObstacleStateManager.RestoreCheckpointState(); 
        ResetTrackedObstacles(); 
    }

    public void ResetCollectedWords()
    {
        foreach (var wordCollector in trackedWords)
        {
            wordCollector.ResetWord();
            wordCollector.gameObject.SetActive(true);
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
    public void ClearCollectedWords()
    {
        collectedWords.Clear();
        selectedWords.Clear();
        UpdateButtons();
        UpdateTopButtons();
        combinationIconManager.ResetCombinationIcon();
        Debug.Log("All collected words have been cleared.");
    }
}