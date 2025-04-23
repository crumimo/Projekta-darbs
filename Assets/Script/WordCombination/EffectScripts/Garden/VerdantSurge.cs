using UnityEngine;

[CreateAssetMenu(fileName = "VerdantSurgeEffect", menuName = "Effects/VerdantSurge")]
public class VerdantSurge : EffectBase
{
    public override void Apply(GameObject target)
    {
        DualObstacle dualObstacle = target.GetComponent<DualObstacle>();
        if (dualObstacle != null)
        {
            dualObstacle.ToggleObstacles(); 
            Debug.Log("VerdantSurgeEffect applied to DualObstacle.");
        }
    }
}