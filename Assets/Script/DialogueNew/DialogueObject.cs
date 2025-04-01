using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private DialogueSegment[] dialogueSegments;
    [SerializeField] private Response[] responses;

    public DialogueSegment[] DialogueSegments => dialogueSegments;

    public bool HasResponses => Responses != null && Responses.Length > 0;

    public Response[] Responses => responses;
}