using UnityEngine;

[CreateAssetMenu(fileName = "NewSceneWords", menuName = "Game/SceneWords")]
public class SceneWordData : ScriptableObject
{
    public WordCombination[] combinations;
}

[System.Serializable]
public class WordCombination
{
    public string word1;
    public string word2;
    public EffectBase effect; 
}