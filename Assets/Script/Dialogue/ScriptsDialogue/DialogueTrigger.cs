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
    private bool playerInRange = false; // New flag to indicate if player is in range

    [SerializeField] private GameObject canTalk;
    [SerializeField] private GameObject cantTalk;

    [SerializeField] private float distanceToActivate;

    private void Update()
    {
        if (playerInRange && (canStartDialogue || requiresNoCombo) && Input.GetKeyDown(KeyCode.F))
        {
            TryStartDialogue();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true; // Set player in range flag

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
            playerInRange = false; // Clear player in range flag

            cantTalk.SetActive(false);
            canTalk.SetActive(false);
        }
    }

    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            Debug.Log($"{effect.GetType().Name} applied to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to DialogueTrigger.");
        }
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