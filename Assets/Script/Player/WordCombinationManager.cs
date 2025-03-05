using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WordCombinationManager : MonoBehaviour
{
    [Header("Panels")]
    public Canvas worldCanvas;
    public Transform playerTransform; // Player's transform

    [Header("Buttons")]
    public Button[] wordButtons; // Buttons for selecting words
    public Button[] topButtons; // Buttons for selected words
    public Button confirmButton; // Button for confirming the combination

    private int topCount = 0; // Counter for selected words
    private Vector3[] originalPositions; // Original positions of word buttons

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
        string[] collectedWords = GetCollectedWords();
        for (int i = 0; i < wordButtons.Length; i++)
        {
            if (i < collectedWords.Length)
            {
                wordButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI buttonText = wordButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = collectedWords[i];
            }
            else
            {
                wordButtons[i].gameObject.SetActive(false);
            }
        }

        // Hide top buttons when updating
        for (int i = 0; i < topButtons.Length; i++)
        {
            topButtons[i].gameObject.SetActive(false);
        }
    }

    // Method to get collected words (stub, replace with your implementation)
    string[] GetCollectedWords()
    {
        return new string[] { "water", "star", "sun", "moon" };
    }

    // Handle confirm button click
    void OnConfirmButtonClick()
    {
        if (topCount == 2)
        {
            string combination = topButtons[0].GetComponentInChildren<TextMeshProUGUI>().text + " " +
                                 topButtons[1].GetComponentInChildren<TextMeshProUGUI>().text;

            Debug.Log("Combination confirmed: " + combination);

            // Add combination handling here

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
                if (button.GetComponentInChildren<TextMeshProUGUI>().text == topButton.GetComponentInChildren<TextMeshProUGUI>().text)
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

            TextMeshProUGUI topButtonText = topButtons[topCount].GetComponentInChildren<TextMeshProUGUI>();
            if (topButtonText == null)
            {
                Debug.LogError("Top button is missing TextMeshProUGUI component!");
                return;
            }

            topButtonText.text = buttonText.text;
            topButtons[topCount].gameObject.SetActive(true);
            button.gameObject.SetActive(false);
            topCount++;

            Debug.Log("Word added to top slot: " + buttonText.text);
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

        foreach (Button button in wordButtons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == topButtonText.text)
            {
                button.gameObject.SetActive(true);
                break;
            }
        }

        Debug.Log("Word removed from top slot: " + topButtonText.text);

        topButtonText.text = "";
        topButton.gameObject.SetActive(false);
        topCount--;
    }

    // Move button with animation (optional)
    IEnumerator MoveButton(Button button, Vector3 targetPosition, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation

        Vector3 startPosition = button.transform.position;

        while (elapsedTime < duration)
        {
            button.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        button.transform.position = targetPosition;
        onComplete?.Invoke();
    }
}