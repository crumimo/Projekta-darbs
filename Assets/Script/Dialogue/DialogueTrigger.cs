using System;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool requiresLullDrift;
    public bool requiresLullThorn;
    private bool canStartDialogue = false;

    [SerializeField] private float distanceToActivate;

   /* private void Start()
    {
        canStartDialogue = true;
    }*/

    void Update()
    {
        if (canStartDialogue && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }
    }

    public void ApplyEffect(string combination)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if (distanceToPlayer > distanceToActivate)
        {
            Debug.Log("Player is too far away to apply the effect.");
            return;
        }

        if ((requiresLullDrift && (combination == "Lull Drift" || combination == "Drift Lull")) ||
            (requiresLullThorn && (combination == "Lull Thorn" || combination == "Thorn Lull")))
        {
            Debug.Log("Combination matched: " + combination);
            canStartDialogue = true;
        }
        else
        {
            Debug.LogWarning("Combination did not match: " + combination);
        }
    }

    private void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        canStartDialogue = false;
    }
}