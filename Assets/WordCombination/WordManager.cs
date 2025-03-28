using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager Instance;

    private List<string> collectedWords = new List<string>();
    private Dictionary<string, string> wordCombinations = new Dictionary<string, string>(); 

    public SceneWordData sceneWordData; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadWordCombinations();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadWordCombinations()
    {
        foreach (var combination in sceneWordData.combinations)
        {
            string combinationKey1 = combination.word1 + "+" + combination.word2;
            string combinationKey2 = combination.word2 + "+" + combination.word1;
            wordCombinations[combinationKey1] = combination.effectName;
            wordCombinations[combinationKey2] = combination.effectName;
        }
    }

    public void CollectWord(string word)
    {
        collectedWords.Add(word);
    }

    public List<string> GetCollectedWords()
    {
        return new List<string>(collectedWords);
    }

    public void SetCollectedWords(List<string> words)
    {
        collectedWords = new List<string>(words);
    }

    public void UseWord(string word)
    {
        if (collectedWords.Contains(word))
        {
            collectedWords.Remove(word);
        }
    }

    public string GetEffect(string word1, string word2)
    {
        if (wordCombinations == null)
        {
            Debug.LogError("wordCombinations is not initialized.");
            return null;
        }

        string combination1 = word1 + "+" + word2;
        string combination2 = word2 + "+" + word1;

        if (wordCombinations.TryGetValue(combination1, out string effect) || wordCombinations.TryGetValue(combination2, out effect))
        {
            return effect;
        }

        return null;
    }
}