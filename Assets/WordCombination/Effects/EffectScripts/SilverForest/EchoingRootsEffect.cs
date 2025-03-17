using UnityEngine;

[CreateAssetMenu(fileName = "EchoingRootsEffect", menuName = "Effects/EchoingRoots")]
public class EchoingRootsEffect : ScriptableObject
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