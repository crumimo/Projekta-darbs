using UnityEngine;

[System.Serializable]
public class DialogueAnswer
{
    public string text;
    [TextArea(3, 10)]
    public string responseText; // Текст, который будет показан после выбора ответа
}