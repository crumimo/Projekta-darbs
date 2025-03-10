using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Canvas dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Transform answersContainer;
    public Button answerButtonPrefab;
    public float typingSpeed = 0.05f;

    private Queue<DialogueSegment> segmentsQueue;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private bool skipTyping = false;

    void Start()
    {
        dialogueCanvas.enabled = false;
        segmentsQueue = new Queue<DialogueSegment>();
    }

    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space)))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else if (segmentsQueue.Count > 0)
            {
                DisplayNextSegment();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.segments.Count == 0)
        {
            Debug.LogError("Передан пустой диалог!");
            return;
        }

        segmentsQueue.Clear();
        foreach (var segment in dialogue.segments)
        {
            segmentsQueue.Enqueue(segment);
        }

        isDialogueActive = true;
        dialogueCanvas.enabled = true;
        FindObjectOfType<Movement>().enabled = false; // Блокируем управление персонажем

        DisplayNextSegment();
    }

    public void DisplayNextSegment()
    {
        if (segmentsQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        var segment = segmentsQueue.Dequeue();
        DisplaySegment(segment);
    }

    public void DisplaySegment(DialogueSegment segment)
    {
        nameText.text = segment.speakerName;

        if (segment.isQuestion)
        {
            DisplayQuestion(segment);
        }
        else
        {
            ClearAnswers();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(segment.sentence));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;

        foreach (char letter in sentence.ToCharArray())
        {
            if (skipTyping)
            {
                dialogueText.text = sentence;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void DisplayQuestion(DialogueSegment segment)
    {
        dialogueText.text = segment.sentence;
        ClearAnswers();

        foreach (var answer in segment.answers)
        {
            Button answerButton = Instantiate(answerButtonPrefab, answersContainer);
            answerButton.GetComponentInChildren<TextMeshProUGUI>().text = answer.text;
            answerButton.onClick.AddListener(() => OnAnswerSelected(answer));
        }
    }

    void ClearAnswers()
    {
        foreach (Transform child in answersContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void OnAnswerSelected(DialogueAnswer answer)
    {
        ClearAnswers();
        StartCoroutine(ShowAnswerAndContinue(answer));
    }

    IEnumerator ShowAnswerAndContinue(DialogueAnswer answer)
    {
        // Показать текст ответа
        yield return StartCoroutine(TypeSentence(answer.responseText));
    
        // Ждем, пока игрок не нажмет кнопку, чтобы продолжить
        while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        // Очищаем ответы после показа текста ответа
        ClearAnswers();

        // Небольшая задержка перед продолжением
        yield return new WaitForSeconds(0.5f);

        // После показа ответа продолжаем основной диалог
        DisplayNextSegment();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas.enabled = false;
        FindObjectOfType<Movement>().enabled = true; // Разблокируем управление персонажем
    }
}