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
        
        var killZone = target.GetComponent<KillZone>();
        if (killZone != null)
        {
            killZone.DisableZone();
            Debug.Log("VerdantSurgeEffect disabled KillZone on " + target.name);
            return;
        }
    }
}