using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WhisperingPetalsEffect", menuName = "Effects/WhisperingPetals")]
public class WhisperingPetalsEffect : EffectBase
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
