using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    public ScriptableObject[] availableEffects; 

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
                    Debug.Log($"InvisibilityEffect applied to {target.name}");
                    return;
                }
                if (effect is SleepEffect sleepEffect)
                {
                    sleepEffect.Apply(target);
                    Debug.Log($"SleepEffect applied to {target.name}");
                    return;
                }
                if (effect is QuietWhisperEffect quietWhisperEffect)
                {
                    quietWhisperEffect.Apply(target);
                    Debug.Log($"QuietWhisperEffect applied to {target.name}");
                    return;
                }
                if (effect is EchoingRootsEffect echoingRootsEffect) 
                {
                    echoingRootsEffect.Apply(target);
                    Debug.Log($"EchoingRootsEffect applied to {target.name}");
                    return;
                }
            }
        }
        Debug.LogWarning($"Effect {effectName} not found!");
    }
}