using UnityEngine;

public class WordCollector : MonoBehaviour
{
    public string word; 
    [SerializeField] private int wordCount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player collected the word: {word}");
            WordUIManager.Instance.CollectWord(word, wordCount);
            WordUIManager.Instance.TrackCollectedWord(this); 
            gameObject.SetActive(false); 
        }
        else
        {
            Debug.Log($"Collided with: {other.name}, but it's not the player.");
        }
    }

    public void ResetWord()
    {
        gameObject.SetActive(true); 
    }
}