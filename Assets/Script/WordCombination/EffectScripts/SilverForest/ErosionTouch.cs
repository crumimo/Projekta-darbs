using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ErosionTouchEffect", menuName = "Effects/ErosionTouchEffect")]
public class ErosionTouchEffect : EffectBase
{
    public override void Apply(GameObject target)
    {
        // Если цель является InteractableObject, применяем эффект
        var interactableObject = target.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            interactableObject.ApplyEffect(this);
            return;
        }

        // Если цель является препятствием, пытаемся уничтожить его
        var obstacleManager = target.GetComponent<ObstacleManager>();
        if (obstacleManager != null && obstacleManager.CanBeDestroyedByEffect(this))
        {
            obstacleManager.DisableObstacle();
        }
    }
}