using UnityEngine;

[CreateAssetMenu(fileName = "ErosionTouchEffect", menuName = "Effects/ErosionTouch")]
public class ErosionTouchEffect : EffectBase
{
    public override void Apply(GameObject target)
    {
        Debug.Log("Applying Erosion Touch Effect");
        ObstacleManager obstacleManager = target.GetComponent<ObstacleManager>();
        if (obstacleManager != null)
        {
            Destroy(target);
            Debug.Log("Obstacle destroyed by Erosion Touch Effect");
        }
    }
}