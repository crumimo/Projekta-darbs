using UnityEngine;
using System.Collections.Generic;

public class WordManager : MonoBehaviour
{
    public static WordManager Instance;
    public SceneWordData sceneWords; // Подключаем ScriptableObject в инспекторе
    private Dictionary<string, string> wordDictionary = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadWords();
    }

    void LoadWords()
    {
        wordDictionary.Clear();
        foreach (var combo in sceneWords.combinations)
        {
            string key1 = combo.word1 + " " + combo.word2;
            string key2 = combo.word2 + " " + combo.word1;
            wordDictionary[key1] = combo.effectName;
            wordDictionary[key2] = combo.effectName;
        }
    }

    public string GetEffect(string word1, string word2)
    {
        string key = word1 + " " + word2;
        if (wordDictionary.TryGetValue(key, out string effect))
        {
            Debug.Log($"Effect found: {effect}");
            return effect;
        }
        Debug.Log("Effect not found, returning 'None'");
        return "None";
    }
}