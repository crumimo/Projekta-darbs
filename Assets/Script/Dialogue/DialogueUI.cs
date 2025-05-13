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
    [SerializeField] private FadeController fadeController; 
    [SerializeField] private DialogueActivator dialogueActivator;

    public event Action OnDialogueEnd;

    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private Dictionary<string, AudioClip[]> voiceDictionary;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        responseHandler.OnResponseSelected += HandleResponseSelected; 

        voiceDictionary = new Dictionary<string, AudioClip[]>();

        CloseDialogueBoxInstant();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        StartCoroutine(ShowDialogueWithFade(dialogueObject));
    }

    private IEnumerator ShowDialogueWithFade(DialogueObject dialogueObject)
    {
        if (dialogueObject.DialogueSegments.Length > 0)
        {
            DialogueSegment firstSegment = dialogueObject.DialogueSegments[0];
            speakerNameLabel.text = firstSegment.speakerName;
        }
    
        yield return StartCoroutine(fadeController.FadeIn());
        dialogueBox.SetActive(true);
        yield return StartCoroutine(fadeController.FadeOut());
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

            yield return RunTypingEffect(segment.sentence, segment.speakerName);
            textLabel.text = segment.sentence;
            
            if (segment.switchNpcToObject && segment.newPrefabToSpawn != null)
            {
                yield return StartCoroutine(dialogueActivator.ReplaceNpcWithObjectWithFade(segment.newPrefabToSpawn));
            }
            else if (segment.switchBackToNpc)
            {
                yield return StartCoroutine(dialogueActivator.RestoreOriginalNpcWithFade());
            }



            if (i == dialogueObject.DialogueSegments.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            StartCoroutine(CloseDialogueBoxWithFade());
        }
    }


    private void HandleResponseSelected(DialogueObject responseDialogueObject)
    {
        StopAllCoroutines();
        StartCoroutine(StepThroughDialogue(responseDialogueObject));
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
        StartCoroutine(CloseDialogueBoxWithFade());
    }

    private IEnumerator CloseDialogueBoxWithFade()
    {
        SoundManager.Instance.EndDialogue();
        yield return StartCoroutine(fadeController.FadeIn());
        
        dialogueBox.SetActive(false);
        
        yield return StartCoroutine(fadeController.FadeOut());

        IsOpen = false;
        textLabel.text = string.Empty;
        speakerNameLabel.text = string.Empty;

        OnDialogueEnd?.Invoke();
    }

    public void CloseDialogueBoxInstant()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        speakerNameLabel.text = string.Empty;
    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
    }
}