using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WordCombinationManager : MonoBehaviour
{
    public static WordCombinationManager Instance;

    [Header("Panels")]
    public Canvas worldCanvas;
    public Transform playerTransform; // Player's transform

    [Header("Buttons")]
    public Button[] wordButtons; // Buttons for selecting words
    public Button[] topButtons; // Buttons for selected words
    public Button confirmButton; // Button for confirming the combination

    private int topCount = 0; // Counter for selected words
    private Vector3[] originalPositions; // Original positions of word buttons
    private Dictionary<string, int> collectedWords = new Dictionary<string, int>(); // Собранные слова и их количество

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Disable the panel at the start of the game
        worldCanvas.enabled = false;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClick);

        // Save original positions of word buttons
        originalPositions = new Vector3[wordButtons.Length];
        for (int i = 0; i < wordButtons.Length; i++)
        {
            originalPositions[i] = wordButtons[i].transform.position;
        }

        // Add listeners to the word buttons
        foreach (Button button in wordButtons)
        {
            AddButtonClickListener(button);
        }

        // Add listeners to the top buttons for right-click removal
        foreach (Button topButton in topButtons)
        {
            topButton.onClick.RemoveAllListeners(); // Remove all previous listeners
            topButton.onClick.AddListener(() => OnTopButtonClick(topButton));
        }

        UpdateButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleWorldPanel();
        }

        // Update the position of the canvas to follow the player
        if (worldCanvas.enabled)
        {
            worldCanvas.transform.position = playerTransform.position + new Vector3(0, 2, 0); // Offset relative to the player
        }

        // Check for right-click on top buttons
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Button topButton in topButtons)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(topButton.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    RemoveWordFromTopButton(topButton);
                    break;
                }
            }
        }
    }

    // Add click listener to word buttons
    void AddButtonClickListener(Button button)
    {
        button.onClick.RemoveAllListeners(); // Remove all previous listeners
        button.onClick.AddListener(() => OnButtonClick(button));
    }

    // Open/close the panel
    void ToggleWorldPanel()
    {
        if (worldCanvas.enabled)
        {
            // If the panel is being closed, reset the selected words
            StartCoroutine(ResetSelectedWords());
        }
        else
        {
            // If the panel is being opened, update the buttons
            UpdateButtons();
        }
        worldCanvas.enabled = !worldCanvas.enabled;
    }

    // Update word buttons
    void UpdateButtons()
    {
        int index = 0;
        foreach (var pair in collectedWords)
        {
            if (index < wordButtons.Length)
            {
                wordButtons[index].gameObject.SetActive(true);
                TextMeshProUGUI buttonText = wordButtons[index].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = $"{pair.Key} x{pair.Value}";
                }
                index++;
            }
        }

        for (int i = index; i < wordButtons.Length; i++)
        {
            wordButtons[i].gameObject.SetActive(false);
        }

        // Hide top buttons when updating
        for (int i = 0; i < topButtons.Length; i++)
        {
            topButtons[i].gameObject.SetActive(false);
        }
    }

    // Method to handle word collection
    public void CollectWord(string word)
    {
        if (collectedWords.ContainsKey(word))
        {
            collectedWords[word]++;
        }
        else
        {
            collectedWords[word] = 1;
        }

        UpdateButtons();
    }

    // Handle confirm button click
    void OnConfirmButtonClick()
    {
        if (topCount == 2)
        {
            // Sort words to ensure order does not matter
            List<string> selectedWords = new List<string>();
            foreach (Button topButton in topButtons)
            {
                TextMeshProUGUI topButtonText = topButton.GetComponentInChildren<TextMeshProUGUI>();
                if (topButtonText != null)
                {
                    selectedWords.Add(topButtonText.text);
                }
            }
            selectedWords.Sort();
            string combination = string.Join(" ", selectedWords);

            Debug.Log("Combination confirmed: " + combination);

            // Decrease the count of used words
            foreach (Button topButton in topButtons)
            {
                TextMeshProUGUI topButtonText = topButton.GetComponentInChildren<TextMeshProUGUI>();
                if (topButtonText != null)
                {
                    string word = topButtonText.text;
                    if (collectedWords.ContainsKey(word) && collectedWords[word] > 0)
                    {
                        collectedWords[word]--;
                        if (collectedWords[word] == 0)
                        {
                            collectedWords.Remove(word);
                        }
                    }
                }
            }

            // Apply the combination effects to enemies
            ApplyCombinationEffect(combination);

            topCount = 0;
            ResetTopButtons();
            UpdateButtons();
            worldCanvas.enabled = false; // Close the canvas after confirmation
        }
        else
        {
            Debug.Log("Not enough words for combination!");
        }
    }

    // Reset top buttons
    void ResetTopButtons()
    {
        for (int i = 0; i < topButtons.Length; i++)
        {
            TextMeshProUGUI topButtonText = topButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (topButtonText != null)
                topButtonText.text = "";
            topButtons[i].gameObject.SetActive(false);
        }
    }

    // Reset selected words without moving slots
    IEnumerator ResetSelectedWords()
    {
        for (int i = 0; i < topCount; i++)
        {
            Button topButton = topButtons[i];
            foreach (Button button in wordButtons)
            {
                if (button.GetComponentInChildren<TextMeshProUGUI>().text.Contains(topButton.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    button.gameObject.SetActive(true);
                }
            }
            topButton.gameObject.SetActive(false);
        }
        topCount = 0;
        yield break;
    }

    // Handle word button click
    public void OnButtonClick(Button button)
    {
        if (topCount < 2)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError("Button is missing TextMeshProUGUI component!");
                return;
            }

            string word = buttonText.text.Split(' ')[0]; // Получить слово без количества

            TextMeshProUGUI topButtonText = topButtons[topCount].GetComponentInChildren<TextMeshProUGUI>();
            if (topButtonText == null)
            {
                Debug.LogError("Top button is missing TextMeshProUGUI component!");
                return;
            }

            topButtonText.text = word;
            topButtons[topCount].gameObject.SetActive(true);
            button.gameObject.SetActive(true);
            topCount++;

            Debug.Log("Word added to top slot: " + word);
        }
        else
        {
            Debug.Log("Top slots are full!");
        }
    }

    // Handle right-click on top button to remove the word
    public void OnTopButtonClick(Button topButton)
    {
        RemoveWordFromTopButton(topButton);
    }

    // Remove word from top button
    void RemoveWordFromTopButton(Button topButton)
    {
        TextMeshProUGUI topButtonText = topButton.GetComponentInChildren<TextMeshProUGUI>();
        if (topButtonText == null || string.IsNullOrEmpty(topButtonText.text))
        {
            Debug.LogError("Top button is missing TextMeshProUGUI component or text is empty!");
            return;
        }

        string word = topButtonText.text;

        foreach (Button button in wordButtons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text.Contains(word))
            {
                button.gameObject.SetActive(true);
                break;
            }
        }

        Debug.Log("Word removed from top slot: " + word);

        topButtonText.text = "";
        topButton.gameObject.SetActive(false);
        topCount--;
    }

    // Apply the combination effects to enemies
    private void ApplyCombinationEffect(string combination)
    {
        // Find all enemies in the scene
        PatrolEnemyEffects[] enemies = FindObjectsOfType<PatrolEnemyEffects>();
        foreach (PatrolEnemyEffects enemy in enemies)
        {
            enemy.ApplyEffect(combination);
        }
    }
}