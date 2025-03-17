using System;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    [SerializeField] private bool requiresNoCombo = false; 
    [SerializeField] private bool requiresQuietWhisper = false; 
    [SerializeField] private bool requiresEchoingRoots = false; 
    private bool canStartDialogue = false;

    [SerializeField] private float distanceToActivate;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key pressed.");
        }

        if ((canStartDialogue || requiresNoCombo) && Input.GetKeyDown(KeyCode.F))
        {
            TryStartDialogue();
        }
    }

    public void ApplyEffect(string effectName)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far away to apply the effect.");
            return;
        }

        Debug.Log("Applying effect: " + effectName);
        EffectManager.Instance.ApplyEffect(effectName, gameObject);
    }

    public void EnableDialogueStart()
    {
        canStartDialogue = true;
        Debug.Log("Dialogue can be started by pressing F.");
    }

    private void TryStartDialogue()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far away to start the dialogue.");
            return;
        }

        StartDialogue();
    }

    private void StartDialogue()
    {
        if (!requiresNoCombo && !canStartDialogue)
        {
            Debug.Log("Combination is required to start this dialogue.");
            return;
        }

        Debug.Log("Starting dialogue: " + dialogue.name);
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(dialogue);
            Debug.Log("Dialogue started.");
        }
        else
        {
            Debug.LogError("DialogueManager not found in the scene.");
        }
        canStartDialogue = false;
    }
}