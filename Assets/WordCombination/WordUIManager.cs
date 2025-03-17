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

    private List<string> selectedWords = new();
    private Dictionary<string, int> collectedWords = new();
    private Dictionary<string, int> savedCollectedWords = new(); 
    private List<WordCollector> trackedWords = new(); 
    private List<WordCollector> checkpointTrackedWords = new(); 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        confirmButton.onClick.AddListener(ConfirmCombination);
        worldCanvas.enabled = false;

        
        foreach (var btn in topButtons)
        {
            btn.onClick.AddListener(() => DeselectWord(btn));
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

        UpdateButtons();
    }

    public void TrackCollectedWord(WordCollector wordCollector)
    {
        trackedWords.Add(wordCollector); 
    }

    public void SaveCheckpoint()
    {
        checkpointTrackedWords = new List<WordCollector>(trackedWords); 
        savedCollectedWords = new Dictionary<string, int>(collectedWords); 
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
            
            worldCanvas.enabled = false;
        }
    }

    void ResetSelection()
    {
        selectedWords.Clear();
        foreach (var btn in topButtons) btn.gameObject.SetActive(false);
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