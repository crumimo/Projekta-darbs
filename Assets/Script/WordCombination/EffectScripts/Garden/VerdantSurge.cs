using UnityEngine;

[CreateAssetMenu(fileName = "VerdantSurgeEffect", menuName = "Effects/VerdantSurge")]
public class VerdantSurge : EffectBase
{
    public override void Apply(GameObject target)
    {
        var dualObstacle = target.GetComponent<DualObstacle>();
        if (dualObstacle != null)
        {
            dualObstacle.ToggleObstacles();
            Debug.Log("VerdantSurgeEffect applied to DualObstacle on " + target.name);
            return;
        }
        
        var killZoneActivator = target.GetComponent<KillZoneActivator>();
        if (killZoneActivator != null)
        {
            killZoneActivator.ApplyEffect(this); 
            Debug.Log("VerdantSurgeEffect applied to KillZoneActivator on " + target.name);
            return;
        }

        Debug.Log("VerdantSurgeEffect: Target " + target.name + " does not have DualObstacle or KillZoneActivator.");
    }
}