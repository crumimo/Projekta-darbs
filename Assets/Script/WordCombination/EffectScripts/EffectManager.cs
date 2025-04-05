using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyEffect(string effectName, GameObject target)
    {
        ScriptableObject effect = Resources.Load<ScriptableObject>($"Effects/{effectName}");

        if (effect == null)
        {
            Debug.LogError($"Effect {effectName} not found!");
            return;
        }

        ApplyEffect(effect, target);
    }
    
    

    public void ApplyEffect(ScriptableObject effect, GameObject target)
    {
        var applyMethod = effect.GetType().GetMethod("Apply");
        if (applyMethod != null)
        {
            applyMethod.Invoke(effect, new object[] { target });
            Debug.Log($"{effect.GetType().Name} applied to {target.name}");
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType().Name} does not have an Apply method or is not applicable to {target.name}.");
        }
    }
}