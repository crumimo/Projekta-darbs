using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EchoVeilEffect", menuName = "Effects/EchoVeil")]
public class EchoVeil : EffectBase
{
    public override void Apply(GameObject target)
    {
        DialogueActivator dialogueActivator = target.GetComponent<DialogueActivator>();
        if (dialogueActivator != null)
        {
            dialogueActivator.EnableDialogueStart();
        }
    }
}
