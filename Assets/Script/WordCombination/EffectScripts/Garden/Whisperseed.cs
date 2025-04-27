using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WhisperseedEffect", menuName = "Effects/Whisperseed")]
public class Whisperseed : EffectBase
{
    public override void Apply(GameObject target)
    {
        var interactableObject = target.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            interactableObject.ApplyEffect(this);
            return;
        }
        
        HideAndSeekEnemyBody body = target.GetComponent<HideAndSeekEnemyBody>();
        if (body != null) 
        {
            body.ApplyCombinationEffect(this);
        }
    }
}
