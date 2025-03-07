using UnityEngine;

public class WordCollector : MonoBehaviour
{
    public string word; // the word that will be collected

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player collected the word: {word}");
            WordCombinationManager.Instance.CollectWord(word);
            Destroy(gameObject); // Destroy the object after collection
        }
        else
        {
            Debug.Log($"Collided with: {other.name}, but it's not the player.");
        }
    }
}