using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f; 
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private DialogueData dialogueData; 

    private Queue<DialogueEntry> sentenceQueue = new Queue<DialogueEntry>();
    private int currentBlockIndex = -1;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isTransitioning = false;

    private void Start()
    {
        StartIntro();
    }

    public void StartIntro()
    {
        sentenceQueue.Clear();
        currentBlockIndex = -1;
        ShowNextBlock();
    }

    private void ShowNextBlock()
    {
        currentBlockIndex++;

        if (currentBlockIndex >= dialogueData.dialogueBlocks.Length)
        {
            EndIntro();
            return;
        }

        DialogueBlock currentBlock = dialogueData.dialogueBlocks[currentBlockIndex];

        if (currentBlock.backgroundImage != null)
        {
            StartCoroutine(ChangeBackgroundSmoothly(currentBlock.backgroundImage, () => ShowNextSentence()));
        }
        else
        {
            ShowNextSentence();
        }

        sentenceQueue.Clear();
        foreach (var sentence in currentBlock.sentences)
        {
            sentenceQueue.Enqueue(sentence);
        }
    }

    private void ShowNextSentence()
    {
        if (sentenceQueue.Count == 0)
        {
            ShowNextBlock();
            return;
        }

        DialogueEntry currentSentence = sentenceQueue.Dequeue();

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        speakerNameText.text = currentSentence.speakerName;
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence.sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private IEnumerator ChangeBackgroundSmoothly(Sprite newBackground, System.Action onComplete)
    {
        isTransitioning = true;
        float duration = fadeSpeed; 
        Color color = backgroundImage.color;
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / duration;
            backgroundImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        backgroundImage.sprite = newBackground;

        alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / duration;
            backgroundImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        backgroundImage.color = new Color(color.r, color.g, color.b, 1f);
        isTransitioning = false;

        onComplete?.Invoke(); // 🔹 Запускаем следующую фразу после смены фона
    }

    private void EndIntro()
    {
        Debug.Log("Intro finished.");
    }

    public void SkipTyping()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            dialogueText.text = sentenceQueue.Peek().sentence;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping && !isTransitioning)
        {
            ShowNextSentence();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isTyping)
        {
            SkipTyping();
        }
    }
}
