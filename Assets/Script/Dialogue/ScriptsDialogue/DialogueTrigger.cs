using System;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    [SerializeField] private bool requiresNoCombo = false; 
    [SerializeField] private bool requiresQuietWhisper = false; 
    [SerializeField] private bool requiresEchoingRoots = false; 
    private bool canStartDialogue = false;

    [SerializeField] private GameObject canTalk;
    [SerializeField] private GameObject cantTalk;

    [SerializeField] private float distanceToActivate;

   

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if ((canStartDialogue || requiresNoCombo) && Input.GetKeyDown(KeyCode.F))
            {
                TryStartDialogue();
            }
            
            if (canStartDialogue || requiresNoCombo)
            {
                canTalk.SetActive(true);
                cantTalk.SetActive(false);
            }

            if (!canStartDialogue)
            {
                cantTalk.SetActive(true);
                canTalk.SetActive(false);
            }
        }
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cantTalk.SetActive(false);
            canTalk.SetActive(false);
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