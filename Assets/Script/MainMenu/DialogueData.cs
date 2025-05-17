using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueBlock[] dialogueBlocks;
}

[System.Serializable]
public class DialogueBlock
{
    public Sprite backgroundImage;
    public DialogueEntry[] sentences;
}

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;
    public string sentence;
}