using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public float distanceToActivate = 10f;

    public void ApplyEffect(ScriptableObject effect)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { gameObject });
            Debug.Log($"{effect.GetType().Name} applied to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to ObstacleManager.");
        }
    }
}