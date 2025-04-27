using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectApplicationManager : MonoBehaviour 
{
    public static EffectApplicationManager Instance;

    [Header("References")]
    public Transform playerTransform;
    public Movement playerMovement;
    public EnemyManager enemyManager;
    public CombinationIconManager combinationIconManager;
    public AudioSource audioSource;
    
    [Header("Effect Settings")]
    public float effectRadius = 10f;

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    /// <summary>
    /// Applies the effect corresponding to the selected word combination.
    /// onEffectApplied is called after the effect is successfully applied.
    /// onEffectFailed is called in case of failure (e.g. if no effect is found or no objects are within range).
    /// </summary>
    public void ApplyCombinationEffect(List<string> selectedWords, Action onEffectApplied, Action onEffectFailed)
    {
        if (selectedWords == null || selectedWords.Count != 2)
        {
            Debug.LogError("Invalid number of words for applying the effect.");
            onEffectFailed?.Invoke();
            return;
        }
        
        string word1 = selectedWords[0];
        string word2 = selectedWords[1];
        
        // Mark the combination as used
        combinationIconManager.MarkCombinationAsUsed(word1, word2);
        
        EffectBase effect = WordManager.Instance.GetEffect(word1, word2);
        AudioClip effectSound = WordManager.Instance.GetEffectSound(word1, word2);
        
        if (effect == null)
        {
            Debug.LogError("Effect not found for the given combination.");
            onEffectFailed?.Invoke();
            return;
        }
        
        bool effectApplied = false;
        
        if (effect is SpikeCircleEffect || effect is GaleStride)
        {
            ApplyEffectToPlayer(effect);
            PlayEffectSound(effectSound);
            effectApplied = true;
        }
        else
        {
            bool objectsInRange = AnyObjectInRange();
            if (objectsInRange)
            {
                effectApplied = ApplyEffectToObjects(effect);
                if (effectApplied)
                    PlayEffectSound(effectSound);
            }
            else
            {
                Debug.Log("No objects within range to apply the effect.");
                onEffectFailed?.Invoke();
                return;
            }
        }
        
        if (effectApplied)
            onEffectApplied?.Invoke();
        else
        {
            Debug.LogWarning("The effect was not applied.");
            onEffectFailed?.Invoke();
        }
        
        // Reset the combination icon display
        combinationIconManager.ResetCombinationIcon();
    }
    
    private void ApplyEffectToPlayer(EffectBase effect)
    {
        effect.Apply(playerTransform.gameObject);
        Debug.Log($"Effect {effect.name} applied to the player");
        // If needed, add fade animation or additional logic here.
    }
    
    private bool ApplyEffectToObjects(EffectBase effect)
    {
        var effectables = FindObjectsOfType<MonoBehaviour>().OfType<IEffectable>();
        bool effectApplied = false;
    
        foreach (var effectable in effectables)
        {
            // Для каждого объекта вызываем его собственную проверку, находится ли игрок в зоне эффекта
            if (effectable.CanReceiveEffect(playerTransform.position, effectRadius, effect))
            {
                effectable.ApplyEffect(effect);
                Debug.Log($"{effect.GetType().Name} applied to {((MonoBehaviour)effectable).name}");
                effectApplied = true;
            }
            else
            {
                Debug.Log($"{((MonoBehaviour)effectable).name} is out of range (using its own logic).");
            }
        }
    
        if (!effectApplied)
            Debug.LogWarning("The effect was not applied to any objects.");
    
        return effectApplied;
    }


    
    private bool AnyObjectInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerTransform.position, effectRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<IEffectable>() != null)
                return true;
        }
        return false;
    }
    
    private void PlayEffectSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
