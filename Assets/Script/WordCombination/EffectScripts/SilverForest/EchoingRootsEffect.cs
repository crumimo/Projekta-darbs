using UnityEngine;

[CreateAssetMenu(fileName = "EchoingRootsEffect", menuName = "Effects/EchoingRoots")]
public class EchoingRootsEffect : EffectBase
{
    private GameObject wordObject; // Reference to the word object
    private Fish fish; // Reference to the Fish instance

    // Set the word object and deactivate it initially
    public void SetWordObject(GameObject word, Transform player)
    {
        wordObject = word;
        wordObject.SetActive(false);
        Debug.Log("Word object set and initially deactivated.");
    }

    // Set the fish instance
    public void SetFish(Fish fishInstance)
    {
        fish = fishInstance;
    }

    // Activate the word object and notify the fish
    public void ActivateWord()
    {
        if (wordObject != null)
        {
            wordObject.SetActive(true);
            Debug.Log("Word object activated.");
            fish?.ApplyCorrectCombination(); // Notify fish to return to original position
        }
        else
        {
            Debug.LogError("Word object is null.");
        }
    }

    public override void Apply(GameObject target)
    {
        // Handle the case for Fish
        Fish fish = target.GetComponent<Fish>();
        if (fish != null)
        {
            Debug.Log("Applying correct combination to fish.");
            fish.ApplyCorrectCombination();
            return; // Exit early since we processed the fish
        }

        // Handle the case for PatrolEnemy
        PatrolEnemy enemy = target.GetComponent<PatrolEnemy>();
        if (enemy != null)
        {
            Debug.Log("Applying effect to PatrolEnemy.");
            enemy.LookAtPlayerAndKill();
            return; // Exit early since we processed the enemy
        }

        // Handle the case for DialogueActivator
        DialogueActivator dialogueActivator = target.GetComponent<DialogueActivator>();
        if (dialogueActivator != null)
        {
            Debug.Log("Enabling dialogue start.");
            dialogueActivator.EnableDialogueStart();
        }
    }
}