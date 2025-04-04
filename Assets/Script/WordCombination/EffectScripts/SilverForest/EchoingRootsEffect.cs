using UnityEngine;

[CreateAssetMenu(fileName = "EchoingRootsEffect", menuName = "Effects/EchoingRoots")]
public class EchoingRootsEffect : EffectBase
{
    public override void Apply(GameObject target)
    {
        PatrolEnemy enemy = target.GetComponent<PatrolEnemy>();
        if (enemy != null)
        {
            enemy.LookAtPlayerAndKill();
        }
        else
        {
            DialogueActivator dialogueActivator = target.GetComponent<DialogueActivator>();
            if (dialogueActivator != null)
            {
                dialogueActivator.EnableDialogueStart();
            }
        }
    }
}