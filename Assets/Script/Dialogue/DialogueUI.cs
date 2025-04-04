using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private TMP_Text speakerNameLabel;
    [SerializeField] private Image speakerImage;
   public event Action OnDialogueEnd;

    public bool IsOpen { get; private set; }
    
    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private Dictionary<string, AudioClip[]> voiceDictionary;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        voiceDictionary = new Dictionary<string, AudioClip[]>();
        
        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.DialogueSegments.Length; i++)
        {
            DialogueSegment segment = dialogueObject.DialogueSegments[i];

            speakerNameLabel.text = segment.speakerName;
            speakerImage.sprite = segment.speakerSprite;

            yield return RunTypingEffect(segment.sentence, segment.speakerName);

            textLabel.text = segment.sentence;
            
            if(i == dialogueObject.DialogueSegments.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private IEnumerator RunTypingEffect(string dialogue, string speakerName)
    {
        typewriterEffect.Run(dialogue, textLabel, speakerName);

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                typewriterEffect.Stop();
            }
        }
    }

    public void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        speakerNameLabel.text = string.Empty;
        speakerImage.sprite = null;
    }
    
    public void EndDialogue()
    {
        // Логика для завершения диалога...
        OnDialogueEnd?.Invoke(); // Вызов события завершения диалога
    }
}


