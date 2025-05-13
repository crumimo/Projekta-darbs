using UnityEngine;

[System.Serializable]
public class DialogueSegment
{
    public string speakerName;
    [TextArea(3, 10)]
    public string sentence;
    public Sprite speakerSprite;
    
    public bool switchNpcToObject;
    public GameObject newPrefabToSpawn;
    public bool switchBackToNpc;

}