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
    public int lettersPerSound = 4; // Звук будет проигрываться раз в 4 буквы
    public Image speakerImage; // Image для отображения спрайта говорящего

    private Queue<DialogueSegment> segmentsQueue;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private bool skipTyping = false;

    public AudioSource audioSource; 
    //public List<CharacterVoice> characterVoices; 
    private Dictionary<string, AudioClip[]> voiceDictionary; 

    void Start()
    {
        dialogueCanvas.enabled = false;
        segmentsQueue = new Queue<DialogueSegment>();
        
        voiceDictionary = new Dictionary<string, AudioClip[]>();
        //foreach (var voice in characterVoices)
        {
            //voiceDictionary[voice.characterName] = voice.voiceClips; 
        }
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
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
        FindObjectOfType<Movement>().enabled = false;

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
        speakerImage.sprite = segment.speakerSprite; // Устанавливаем спрайт говорящего

        //if (segment.isQuestion)
        {
            DisplayQuestion(segment);
        }
        //else
        {
            ClearAnswers();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(segment.sentence, segment.speakerName));
        }
    }

    IEnumerator TypeSentence(string sentence, string speakerName)
    {
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;

        for (int i = 0; i < sentence.Length; i++)
        {
            if (skipTyping)
            {
                dialogueText.text = sentence;
                break;
            }

            dialogueText.text += sentence[i];

            // Воспроизводим звук раз в каждые 4 буквы
            if (i % lettersPerSound == 0 && voiceDictionary.ContainsKey(speakerName) && audioSource != null)
            {
                AudioClip[] sounds = voiceDictionary[speakerName];
                if (sounds.Length > 0)
                {
                    audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void DisplayQuestion(DialogueSegment segment)
    {
        dialogueText.text = segment.sentence;
        ClearAnswers();

        //foreach (var answer in segment.answers)
        {
            //Button answerButton = Instantiate(answerButtonPrefab, answersContainer);
            //answerButton.GetComponentInChildren<TextMeshProUGUI>().text = answer.text;
            //answerButton.onClick.AddListener(() => OnAnswerSelected(answer));
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
        yield return StartCoroutine(TypeSentence(answer.responseText, nameText.text));

        while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        ClearAnswers();
        yield return new WaitForSeconds(0.5f);
        DisplayNextSegment();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas.enabled = false;
        FindObjectOfType<Movement>().enabled = true;
    }
}

//[System.Serializable]
//public class CharacterVoice

    //public string characterName;
    //public AudioClip[] voiceClips; 
