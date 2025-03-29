using UnityEngine;

public class FishBehavior : EffectBase
{
    private GameObject wordObject; // Reference to the word object
    private Transform playerTransform; // Reference to the player's transform
    private Fish fish; // Reference to the Fish instance

    public void SetWordObject(GameObject word, Transform player)
    {
        wordObject = word;
        wordObject.SetActive(false); // Ensure the word object is initially inactive
        playerTransform = player;
    }

    public void SetFish(Fish fishInstance)
    {
        fish = fishInstance;
    }

    public void ActivateWord()
    {
        if (wordObject != null)
        {
            wordObject.SetActive(true);
            fish.StartMovingToPlayer(); // Start moving the fish to the player
        }
    }

    public override void Apply(GameObject target)
    {
        Fish fish = target.GetComponent<Fish>();
        if (fish != null)
        {
            fish.ApplyCorrectCombination();
        }
    }
}