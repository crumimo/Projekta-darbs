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
                if (effect is InvisibilityEffect invisibilityEffect && target.GetComponent<PatrolEnemy>() != null)
                {
                    invisibilityEffect.Apply(target);
                    Debug.Log($"InvisibilityEffect applied to {target.name}");
                    return;
                }
                if (effect is SleepEffect sleepEffect && target.GetComponent<PatrolEnemy>() != null)
                {
                    sleepEffect.Apply(target);
                    Debug.Log($"SleepEffect applied to {target.name}");
                    return;
                }
                if (effect is QuietWhisperEffect quietWhisperEffect && target.GetComponent<DialogueTrigger>() != null)
                {
                    quietWhisperEffect.Apply(target);
                    Debug.Log($"QuietWhisperEffect applied to {target.name}");
                    return;
                }
                if (effect is EchoingRootsEffect echoingRootsEffect && target.GetComponent<DialogueTrigger>() != null)
                {
                    echoingRootsEffect.Apply(target);
                    Debug.Log($"EchoingRootsEffect applied to {target.name}");
                    return;
                }
                if (effect is ThornDriftEffect thornDriftEffect && target.CompareTag("Player"))
                {
                    thornDriftEffect.Apply(target);
                    Debug.Log($"ThornDriftEffect applied to {target.name}");
                    return;
                }
            }
        }
        Debug.LogWarning($"Effect {effectName} not found or not applicable to {target.name}!");
    }
}