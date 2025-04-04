using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager Instance;

    private List<string> collectedWords = new List<string>();
    private Dictionary<string, EffectBase> wordCombinations = new Dictionary<string, EffectBase>(); 

    public SceneWordData[] sceneWordDataArray; 

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
        foreach (var sceneWordData in sceneWordDataArray)
        {
            foreach (var combination in sceneWordData.combinations)
            {
                string combinationKey1 = combination.word1 + "+" + combination.word2;
                string combinationKey2 = combination.word2 + "+" + combination.word1;
                wordCombinations[combinationKey1] = combination.effect;
                wordCombinations[combinationKey2] = combination.effect;
            }
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

    public EffectBase GetEffect(string word1, string word2)
    {
        if (wordCombinations == null)
        {
            Debug.LogError("wordCombinations is not initialized.");
            return null;
        }

        string combination1 = word1 + "+" + word2;
        string combination2 = word2 + "+" + word1;

        if (wordCombinations.TryGetValue(combination1, out EffectBase effect) || wordCombinations.TryGetValue(combination2, out effect))
        {
            return effect;
        }

        return null;
    }
}