using UnityEngine;

public class FishBehavior : EffectBase
{
    private GameObject wordObject; // Reference to the word object
    private Fish fish; // Reference to the Fish instance

    public void SetWordObject(GameObject word, Transform player)
    {
        wordObject = word;
        wordObject.SetActive(false); // Ensure the word object is initially inactive
        Debug.Log("Word object set and initially deactivated.");
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
            Debug.Log("Word object activated.");
            fish.ApplyCorrectCombination(); // Set the flag to return to the original position
        }
        else
        {
            Debug.LogError("Word object is null.");
        }
    }

    public override void Apply(GameObject target)
    {
        Fish fish = target.GetComponent<Fish>();
        if (fish != null)
        {
            Debug.Log("Applying correct combination to fish.");
            fish.ApplyCorrectCombination();
        }
        else
        {
            //Debug.LogError("Fish component not found on target.");
        }
    }
}