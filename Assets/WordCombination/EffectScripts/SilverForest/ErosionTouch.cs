using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ErosionTouchEffect", menuName = "Effects/ErosionTouchEffect")]
public class ErosionTouchEffect : EffectBase
{
    public override void Apply(GameObject target)
    {
        var obstacleManager = target.GetComponent<ObstacleManager>();
        if (obstacleManager != null && obstacleManager.CanBeDestroyedByEffect(this))
        {
            obstacleManager.DisableObstacle();
        }
    }
}