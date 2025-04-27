using UnityEngine;

[CreateAssetMenu(fileName = "HollowNestEffect", menuName = "Effects/HollowNest")]
public class HollowNest : EffectBase
{
    public override void Apply(GameObject target)
    {
        var nestController = target.GetComponent<HollowNestController>();
        if(nestController != null)
        {
            nestController.ActivateNest();
            Debug.Log("HollowNestEffect applied to " + target.name);
        }
    }
}