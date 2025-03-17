using System;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool requiresCombo = false; // Flag to check if combination is required
    private bool canStartDialogue = false;

    [SerializeField] private float distanceToActivate;

    void Update()
    {
        if (canStartDialogue && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far away to start the dialogue.");
            return;
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        canStartDialogue = false;
    }

    public void EnableDialogueStart()
    {
        canStartDialogue = true;
    }
}