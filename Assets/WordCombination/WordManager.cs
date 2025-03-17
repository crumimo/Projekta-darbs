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
            string combinationKey = combination.word1 + "+" + combination.word2;
            wordCombinations[combinationKey] = combination.effectName;
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

        string combination = word1 + "+" + word2;
        if (wordCombinations.TryGetValue(combination, out string effect))
        {
            return effect;
        }

        return null;
    }
}