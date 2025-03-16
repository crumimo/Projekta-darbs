using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    public ScriptableObject[] availableEffects; // Массив доступных эффектов

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyEffect(string effectName, GameObject target)
    {
        Debug.Log($"Applying effect: {effectName}");
        foreach (var effect in availableEffects)
        {
            if (effect.name == effectName)
            {
                if (effect is InvisibilityEffect invisibilityEffect)
                {
                    invisibilityEffect.Apply(target);
                    return;
                }
                else if (effect is SleepEffect sleepEffect)
                {
                    sleepEffect.Apply(target);
                    return;
                }
            }
        }
        Debug.LogWarning($"Effect {effectName} not found!");
    }
}