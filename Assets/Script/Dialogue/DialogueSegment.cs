using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueSegment
{
    public string speakerName;
    public bool isQuestion;
    
    [TextArea(3, 10)]
    public string sentence;

    [NonReorderable]
    public List<DialogueAnswer> answers;
}