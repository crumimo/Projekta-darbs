using UnityEngine;

[CreateAssetMenu(fileName = "QuietWhisperEffect", menuName = "Effects/QuietWhisper")]
public class QuietWhisperEffect : ScriptableObject
{
    public void Apply(GameObject target)
    {
        PatrolEnemy enemy = target.GetComponent<PatrolEnemy>();
        if (enemy != null)
        {
            enemy.LookAtPlayerAndKill();
        }
        else
        {
            DialogueTrigger dialogueTrigger = target.GetComponent<DialogueTrigger>();
            if (dialogueTrigger != null)
            {
                dialogueTrigger.EnableDialogueStart();
            }
        }
    }
}