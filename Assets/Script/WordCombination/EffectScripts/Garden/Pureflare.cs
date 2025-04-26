using UnityEngine;

[CreateAssetMenu(fileName = "PureflareEffect", menuName = "Effects/Pureflare")]
public class Pureflare : EffectBase 
{
    public override void Apply(GameObject target)
    {
        HideAndSeekEnemyBody body = target.GetComponent<HideAndSeekEnemyBody>();
        if (body != null) 
        {
            body.ApplyCombinationEffect(this);
        }
        
        var obstacleManager = target.GetComponent<ObstacleManager>();
        if (obstacleManager != null && obstacleManager.CanBeDestroyedByEffect(this))
        {
            obstacleManager.DisableObstacle();
        }
    }
}