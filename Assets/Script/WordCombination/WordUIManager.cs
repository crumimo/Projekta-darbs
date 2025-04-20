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

    [Header("Image Display")]
    public Image combinationImage;
    private Coroutine imageFadeCoroutine;
    
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
        combinationImage.gameObject.SetActive(false); 
        
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
            if (!checkpointTrackedWords.Contains(wordCollector))
            {
                wordCollector.gameObject.SetActive(true); 
            }
        }
        trackedWords = new List<WordCollector>(checkpointTrackedWords);
        
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
    
    IEnumerator FadeInImage(Image image, float duration = 0.5f)
    {
        image.gameObject.SetActive(true); 
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Clamp01(elapsedTime / duration); 
            image.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        image.color = color; 
        imageFadeCoroutine = null; 
    }

    IEnumerator FadeOutImage(Image image, float duration = 0.5f)
    {
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            color.a = 1f - Mathf.Clamp01(elapsedTime / duration); 
            image.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        image.color = color; 
        image.gameObject.SetActive(false); 
        imageFadeCoroutine = null; 
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
                
                if (selectedWords.Count == 2)
                {
                    DisplayCombinationImage(); 
                }
            }
            
        }
        
    }
    
    void DisplayCombinationImage()
    {
        string word1 = selectedWords[0];
        string word2 = selectedWords[1];
        Sprite combinationSprite = WordManager.Instance.GetCombinationSprite(word1, word2);

        if (combinationSprite != null)
        {
            combinationImage.sprite = combinationSprite;
            
            if (imageFadeCoroutine != null)
            {
                StopCoroutine(imageFadeCoroutine); 
            }

            imageFadeCoroutine = StartCoroutine(FadeInImage(combinationImage));
        }
        else
        {
            Debug.LogWarning($"No sprite found for combination: {word1} + {word2}");
            
            if (imageFadeCoroutine != null)
            {
                StopCoroutine(imageFadeCoroutine); 
            }

            imageFadeCoroutine = StartCoroutine(FadeOutImage(combinationImage));
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
        
        if (selectedWords.Count == 2)
        {
            DisplayCombinationImage(); 
        }
        else
        {
            if (imageFadeCoroutine != null)
            {
                StopCoroutine(imageFadeCoroutine); 
            }

            imageFadeCoroutine = StartCoroutine(FadeOutImage(combinationImage)); 
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
        playerVisual.sortingLayerName = "Middleground";
        playerVisual.sortingOrder = 0;

        EffectBase effect = WordManager.Instance.GetEffect(selectedWords[0], selectedWords[1]);
        AudioClip effectSound = WordManager.Instance.GetEffectSound(selectedWords[0], selectedWords[1]);

        if (effect == null)
        {
            Debug.LogError("Effect not found for the given combination.");
            return;
        }

        Debug.Log($"Confirmed combination: {selectedWords[0]} + {selectedWords[1]} = {effect.name}");

        bool effectApplied = false;

        
        if (effect is SpikeCircleEffect || effect is GaleStrideEffect || effect is FlourishingVeilEffect)
        {
            ApplyEffectToPlayer(effect);
            PlayEffectSound(effectSound);
            effectApplied = true;
        }
        else
        {
            bool objectsInRange = AnyObjectInRange();
            if (objectsInRange)
            {
                effectApplied = ApplyEffectToObjects(effect);
                if (effectApplied)
                {
                    PlayEffectSound(effectSound);
                }
            }
            else
            {
                Debug.Log("No objects in range to apply the effect.");
                ReturnWordsToCollection();
            }
        }

        if (effectApplied)
        {
            
            StartCoroutine(FadeOut(worldCanvasGroup, 0.5f, () =>
            {
                worldCanvas.enabled = false;
                playerMovement.EnableMovement();
                enemyManager.ResumeEnemies();
            }));
        }
        else
        {
            Debug.LogWarning("Effect was not applied. Panel remains open.");
        }

        ResetSelection();
    }
}

private void ApplyEffectToPlayer(EffectBase effect)
{
    effect.Apply(playerTransform.gameObject);
    Debug.Log($"Applying {effect.name} to Player");

    StartCoroutine(FadeOut(worldCanvasGroup, 0.5f, () =>
    {
        worldCanvas.enabled = false;
        playerMovement.EnableMovement();
        enemyManager.ResumeEnemies();
    }));
}

private bool ApplyEffectToObjects(EffectBase effect)
{
    var effectables = FindObjectsOfType<MonoBehaviour>().OfType<IEffectable>();
    bool effectApplied = false;

    foreach (var effectable in effectables)
    {
        // Применяем выбранный эффект
        if (Vector3.Distance(playerTransform.position, ((MonoBehaviour)effectable).transform.position) <= effectRadius)
        {
            effectable.ApplyEffect(effect);
            Debug.Log($"{effect.GetType().Name} applied to {((MonoBehaviour)effectable).name}.");
            effectApplied = true;
        }
    }

    if (!effectApplied)
    {
        Debug.LogWarning("Effect could not be applied to any object.");
        ReturnWordsToCollection(); 
    }

    return effectApplied;
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
        combinationImage.gameObject.SetActive(false);
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
                        btn.gameObject.SetActive(true); // Ensure button is active
                    }
                }
            }
        }
    }

    public void ResetToCheckpoint()
    {
        RestoreCollectedWords();
        RestoreCollectedWordsOnScene();
        ObstacleStateManager.RestoreCheckpointState(); // Restore obstacles to the checkpoint state
        ResetTrackedObstacles(); // Reset tracked obstacles
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
}